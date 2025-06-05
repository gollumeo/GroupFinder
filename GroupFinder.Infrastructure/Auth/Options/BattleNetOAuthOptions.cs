namespace GroupFinder.Infrastructure.Auth.Options;

public class BattleNetOAuthOptions
{
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    
    public string AuthorizationUrl { get; init; } = null!;
    public string TokenUrl { get; init; } = null!;
    public string UserInfoUrl { get; init; } = null!;
    
    public string RedirectUri { get; init; } = null!;
    public string Scope { get; init; } = null!;
    public string ResponseType { get; init; } = null!;
}