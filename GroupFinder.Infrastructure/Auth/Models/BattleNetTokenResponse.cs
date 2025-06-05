namespace GroupFinder.Infrastructure.Auth.Models;

public sealed class BattleNetTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string TokenType { get; set; } = null!;
    public int ExpiresIn { get; set; }
    public string Scope { get; set; } = null!;
}