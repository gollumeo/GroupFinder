using GroupFinder.Application.Auth.ValueObjects;

namespace GroupFinder.Application.Auth.Contracts;

public interface IBattleNetOauth
{
    string GetLoginUrl(string redirectUri, string state);
    Task<BattleNetUserInfo> ExchangeCodeAsync(string code, string redirectUri);
}