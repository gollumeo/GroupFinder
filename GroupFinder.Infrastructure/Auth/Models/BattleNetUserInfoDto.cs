namespace GroupFinder.Infrastructure.Auth.Models;

public sealed class BattleNetUserInfoDto
{
    public string Id { get; init; } = null!;
    public string BattleTag { get; init; } = null!;
    public string Region { get; init; } = null!;
}