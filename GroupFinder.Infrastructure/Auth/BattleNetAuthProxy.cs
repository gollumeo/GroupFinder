using System.Text.Json;
using System.Web;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;
using GroupFinder.Infrastructure.Auth.Models;
using GroupFinder.Infrastructure.Auth.Options;
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

            return Result<ExternalUserInfo>.Failure("Access token acquired, user info not yet fetched.");
        }
        catch (Exception ex)
        {
            return Result<ExternalUserInfo>.Failure($"Exception during token exchange: {ex.Message}");
        }
    }
}