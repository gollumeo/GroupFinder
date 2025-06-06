using System.Net.Http.Headers;
using System.Text.Json;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;
using GroupFinder.Infrastructure.Auth.Contracts;
using GroupFinder.Infrastructure.Auth.Models;
using GroupFinder.Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;

namespace GroupFinder.Infrastructure.Auth;

public class BattleNetApiClient(HttpClient httpClient, IOptions<BattleNetOAuthOptions> options) : IBattleNetApi
{
    private readonly BattleNetOAuthOptions _options = options.Value;
    
    public async Task<BattleNetTokenResponse> ExchangeCodeForToken(string code, string redirectUri)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret }
        };

        var response = await httpClient.PostAsync(_options.TokenUrl, new FormUrlEncodedContent(tokenRequest));
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<BattleNetTokenResponse>(json);

        return result!;
    }

    public async Task<BattleNetUserInfoDto> FetchUserInfo(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _options.UserInfoUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<BattleNetUserInfoDto>(json);

        return dto!;
    }
}