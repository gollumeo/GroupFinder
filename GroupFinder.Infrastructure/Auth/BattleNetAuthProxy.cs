using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;
using GroupFinder.Infrastructure.Auth.Models;
using GroupFinder.Infrastructure.Auth.Options;
using GroupFinder.Infrastructure.Auth.ValueObjects;
using Microsoft.Extensions.Options;

namespace GroupFinder.Infrastructure.Auth;

public class BattleNetAuthProxy(
    HttpClient httpClient,
    IOptions<BattleNetOAuthOptions> options
) : IExternalAuthentication
{
    private readonly BattleNetOAuthOptions _options = options.Value;
    private readonly HttpClient _httpClient = httpClient;

    public string GetLoginUrl(string redirectUri, string state)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = _options.ClientId;
        query["redirect_uri"] = redirectUri;
        query["response_type"] = _options.ResponseType;
        query["scope"] = _options.Scope;
        query["state"] = state;

        return $"{_options.AuthorizationUrl}?{query}";
    }

    public async Task<Result<ExternalUserInfo>> ExchangeCodeAsync(string code, string redirectUri)
    {
        try
        {
            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret }
            };

            var tokenResponse = await _httpClient.PostAsync(_options.TokenUrl, new FormUrlEncodedContent(tokenRequest));

            if (!tokenResponse.IsSuccessStatusCode)
                return Result<ExternalUserInfo>.Failure("Battle.net token request failed.");

            var json = await tokenResponse.Content.ReadAsStringAsync();

            var token = JsonSerializer.Deserialize<BattleNetTokenResponse>(json);

            if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
                return Result<ExternalUserInfo>.Failure("Invalid token response from Battle.net.");

            var userRequest = new HttpRequestMessage(HttpMethod.Get, _options.UserInfoUrl);
            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var userResponse = await _httpClient.SendAsync(userRequest);

            if (!userResponse.IsSuccessStatusCode)
                return Result<ExternalUserInfo>.Failure(
                    $"Failed to fetch user info. Status: {userResponse.StatusCode}");

            var userJson = await userResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<BattleNetUserInfoDto>(userJson);

            if (userInfo is null)
                return Result<ExternalUserInfo>.Failure("Invalid user info response from Battle.net.");

            var region = "eu";
            var internalUser = BattleNetUserInfo.Create(userInfo.Id, userInfo.BattleTag, region);

            if (internalUser.IsFailure)
                return Result<ExternalUserInfo>.Failure(internalUser.Error);

            var result = new ExternalUserInfo(
                internalUser.Value.Id,
                internalUser.Value.BattleTag,
                internalUser.Value.Region
            );
            
            return Result<ExternalUserInfo>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ExternalUserInfo>.Failure($"Exception during token exchange: {ex.Message}");
        }
    }
}