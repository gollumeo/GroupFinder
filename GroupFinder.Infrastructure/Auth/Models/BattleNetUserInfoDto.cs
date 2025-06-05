using System.Text.Json.Serialization;

namespace GroupFinder.Infrastructure.Auth.Models;

public sealed class BattleNetUserInfoDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    
    [JsonPropertyName("battletag")]
    public string BattleTag { get; init; } = null!;
    
    [JsonPropertyName("region")]
    public string Region { get; init; } = null!;
}