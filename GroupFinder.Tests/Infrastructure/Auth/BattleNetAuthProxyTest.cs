using System.Net;
using System.Text;
using System.Web;
using FluentAssertions;
using GroupFinder.Infrastructure.Auth;
using GroupFinder.Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace GroupFinder.Tests.Infrastructure.Auth;

public class BattleNetAuthProxyTests
{
    private const string ValidTokenJson =
        "{ \"access_token\": \"abc123\", \"token_type\": \"bearer\", \"expires_in\": 3600, \"scope\": \"wow.profile\" }";

    private const string ValidUserJson = "{ \"id\": \"user123\", \"battletag\": \"TestUser#1234\" }";

    private static BattleNetOAuthOptions FakeOptions => new()
    {
        ClientId = "fake-client-id",
        ClientSecret = "fake-client-secret",
        AuthorizationUrl = "https://fake.auth.com/oauth/authorize",
        TokenUrl = "https://fake.auth.com/oauth/token",
        UserInfoUrl = "https://fake.api.com/userinfo",
        RedirectUri = "https://myapp.com/callback",
        Scope = "wow.profile",
        ResponseType = "code"
    };

    private static HttpClient CreateMockHttpClient(HttpResponseMessage tokenResponse,
        HttpResponseMessage userInfoResponse)
    {
        var handler = new Mock<HttpMessageHandler>();

        handler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(tokenResponse)
            .ReturnsAsync(userInfoResponse);

        return new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("https://fake.api.com")
        };
    }

    [Fact]
    public async Task ExchangeCodeAsyncReturnsSuccessWithValidTokenAndUserInfo()
    {
        var tokenResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ValidTokenJson, Encoding.UTF8, "application/json")
        };

        var userInfoResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ValidUserJson, Encoding.UTF8, "application/json")
        };

        var client = CreateMockHttpClient(tokenResponse, userInfoResponse);
        var options = Options.Create(FakeOptions);

        // TODO: fix compil & mocks
        var proxy = new BattleNetAuthProxy(client, options);

        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("user123");
        result.Value.BattleTag.Should().Be("TestUser#1234");
        result.Value.Region.Should().Be("eu");
    }

    [Fact]
    public async Task ExchangeCodeAsyncReturnsFailureWhenTokenCallFails()
    {
        var tokenResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var client = CreateMockHttpClient(tokenResponse, new HttpResponseMessage());

        var proxy = new BattleNetAuthProxy(client, Options.Create(FakeOptions));
        var result = await proxy.ExchangeCodeAsync("bad_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("token request failed");
    }

    [Fact]
    public async Task ExchangeCodeAsyncReturnsFailureWhenTokenJsonIsMalformed()
    {
        var tokenResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ \"access_token_missing\": \"abc123\" }", Encoding.UTF8, "application/json")
        };
        var userInfoResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ValidUserJson, Encoding.UTF8, "application/json")
        };

        var client = CreateMockHttpClient(tokenResponse, userInfoResponse);
        var proxy = new BattleNetAuthProxy(client, Options.Create(FakeOptions));
        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid token response");
    }

    [Fact]
    public async Task ExchangeCodeAsyncReturnsFailureWhenUserInfoFails()
    {
        var tokenResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ValidTokenJson, Encoding.UTF8, "application/json")
        };

        var userInfoResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var client = CreateMockHttpClient(tokenResponse, userInfoResponse);

        var proxy = new BattleNetAuthProxy(client, Options.Create(FakeOptions));
        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to fetch user info");
    }

    [Fact]
    public async Task ExchangeCodeAsyncReturnsFailureWithInvalidUserInfo()
    {
        var tokenResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ValidTokenJson, Encoding.UTF8, "application/json")
        };

        var userInfoResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        var client = CreateMockHttpClient(tokenResponse, userInfoResponse);
        var proxy = new BattleNetAuthProxy(client, Options.Create(FakeOptions));

        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("BattleNetUserInfo: id is required.");
    }

    [Fact]
    public void GetLoginUrl_ComposesUrlWithAllQueryParameters()
    {
        var options = Options.Create(FakeOptions);
        var client = new HttpClient();
        var proxy = new BattleNetAuthProxy(client, options);

        const string redirectUri = "https://myapp.com/callback";
        const string state = "someRandomState123";

        var url = proxy.GetLoginUrl(redirectUri, state);

        url.Should().StartWith(FakeOptions.AuthorizationUrl);

        var uri = new Uri(url);
        var query = HttpUtility.ParseQueryString(uri.Query);

        query["client_id"].Should().Be(FakeOptions.ClientId);
        query["redirect_uri"].Should().Be(redirectUri);
        query["response_type"].Should().Be(FakeOptions.ResponseType);
        query["scope"].Should().Be(FakeOptions.Scope);
        query["state"].Should().Be(state);
    }

    [Fact]
    public void GetLoginUrl_EncodesQueryParameters()
    {
        var options = Options.Create(FakeOptions);
        var client = new HttpClient();
        var proxy = new BattleNetAuthProxy(client, options);

        const string redirectUri = "https://myapp.com/callback?param=value&other=éàè";
        const string state = "spécial state&with chars";

        var url = proxy.GetLoginUrl(redirectUri, state);

        var uri = new Uri(url);
        var query = HttpUtility.ParseQueryString(uri.Query);

        uri.GetLeftPart(UriPartial.Path).Should().Be("https://fake.auth.com/oauth/authorize");
        query["redirect_uri"].Should().Be(redirectUri);
        query["state"].Should().Be(state);
    }

    [Fact]
    public void GetLoginUrl_HandlesNullOrEmptyState()
    {
        var options = Options.Create(FakeOptions);
        var client = new HttpClient();
        var proxy = new BattleNetAuthProxy(client, options);

        var urlWithNullState = proxy.GetLoginUrl(FakeOptions.RedirectUri, null!);
        var urlWithEmptyState = proxy.GetLoginUrl(FakeOptions.RedirectUri, "");

        var uriNull = new Uri(urlWithNullState);
        var queryNull = HttpUtility.ParseQueryString(uriNull.Query);

        var uriEmpty = new Uri(urlWithEmptyState);
        var queryEmpty = HttpUtility.ParseQueryString(uriEmpty.Query);

        queryNull["client_id"].Should().Be(FakeOptions.ClientId);
        queryNull["state"].Should().BeNullOrEmpty();

        queryEmpty["client_id"].Should().Be(FakeOptions.ClientId);
        queryEmpty["state"].Should().BeNullOrEmpty();
    }
}