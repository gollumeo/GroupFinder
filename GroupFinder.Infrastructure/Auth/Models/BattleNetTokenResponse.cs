using System.Text.Json.Serialization;

namespace GroupFinder.Infrastructure.Auth.Models;

public sealed class BattleNetTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = null!;
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = null!;
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    
    [JsonPropertyName("scope")]
    public string Scope { get; init; } = null!;
}