using System.Web;
using FluentAssertions;
using GroupFinder.Infrastructure.Auth;
using GroupFinder.Infrastructure.Auth.Contracts;
using GroupFinder.Infrastructure.Auth.Models;
using GroupFinder.Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;
using Moq;

namespace GroupFinder.Tests.Infrastructure.Auth;

public class BattleNetAuthProxyTests
{
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

    private static IOptions<BattleNetOAuthOptions> Options => Microsoft.Extensions.Options.Options.Create(FakeOptions);

    [Fact]
    public async Task AuthenticatesSuccessfullyWithValidTokenAndUser()
    {
        var api = new Mock<IBattleNetApi>();
        api.Setup(x => x.ExchangeCodeForToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new BattleNetTokenResponse
                { AccessToken = "abc123", TokenType = "bearer", ExpiresIn = 3600, Scope = "wow.profile" });
        api.Setup(x => x.FetchUserInfo(It.IsAny<string>()))
            .ReturnsAsync(new BattleNetUserInfoDto { Id = "user123", BattleTag = "TestUser#1234" });

        var proxy = new BattleNetAuthProxy(api.Object, Options);
        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("user123");
        result.Value.BattleTag.Should().Be("TestUser#1234");
        result.Value.Region.Should().Be("eu");
    }

    [Fact]
    public async Task FailsAuthenticationWhenTokenEndpointThrows()
    {
        var api = new Mock<IBattleNetApi>();
        api.Setup(x => x.ExchangeCodeForToken(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("token request failed"));

        var proxy = new BattleNetAuthProxy(api.Object, Options);
        var result = await proxy.ExchangeCodeAsync("bad_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("token request failed");
    }

    [Fact]
    public async Task FailsAuthenticationWhenTokenIsMalformed()
    {
        var api = new Mock<IBattleNetApi>();
        api.Setup(x => x.ExchangeCodeForToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new BattleNetTokenResponse { AccessToken = null! });
        api.Setup(x => x.FetchUserInfo(It.IsAny<string>()))
            .ReturnsAsync(new BattleNetUserInfoDto { Id = "user123", BattleTag = "TestUser#1234" });

        var proxy = new BattleNetAuthProxy(api.Object, Options);
        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task FailsAuthenticationWhenUserInfoThrows()
    {
        var api = new Mock<IBattleNetApi>();
        api.Setup(x => x.ExchangeCodeForToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new BattleNetTokenResponse { AccessToken = "abc123" });
        api.Setup(x => x.FetchUserInfo(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Failed to fetch user info"));

        var proxy = new BattleNetAuthProxy(api.Object, Options);
        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to fetch user info");
    }

    [Fact]
    public async Task FailsAuthenticationWhenUserInfoIsInvalid()
    {
        var api = new Mock<IBattleNetApi>();
        api.Setup(x => x.ExchangeCodeForToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new BattleNetTokenResponse { AccessToken = "abc123" });
        api.Setup(x => x.FetchUserInfo(It.IsAny<string>()))
            .ReturnsAsync(new BattleNetUserInfoDto());

        var proxy = new BattleNetAuthProxy(api.Object, Options);
        var result = await proxy.ExchangeCodeAsync("valid_code", FakeOptions.RedirectUri);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("BattleNetUserInfo: id is required.");
    }

    [Fact]
    public void ComposesOAuthUrlCorrectly()
    {
        var api = new Mock<IBattleNetApi>();
        var proxy = new BattleNetAuthProxy(api.Object, Options);

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
    public void EncodesOAuthUrlQueryParameters()
    {
        var api = new Mock<IBattleNetApi>();
        var proxy = new BattleNetAuthProxy(api.Object, Options);

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
    public void HandlesNullOrEmptyStateInOAuthUrl()
    {
        var api = new Mock<IBattleNetApi>();
        var proxy = new BattleNetAuthProxy(api.Object, Options);

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