using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.ValueObjects;

namespace GroupFinder.Infrastructure.Auth;

public class BattleNetOAuth : IBattleNetOAuth
{
    public string GetLoginUrl(string redirectUri, string state)
    {
        throw new NotImplementedException();
    }

    public Task<BattleNetUserInfo> ExchangeCodeAsync(string code, string redirectUri)
    {
        throw new NotImplementedException();
    }
}