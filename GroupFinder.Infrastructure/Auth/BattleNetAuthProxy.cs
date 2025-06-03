using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;
using GroupFinder.Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;

namespace GroupFinder.Infrastructure.Auth;

public class BattleNetAuthProxy(IOptions<BattleNetOAuthOptions> options) : IExternalAuthentication
{
    private readonly BattleNetOAuthOptions _options = options.Value;

    public string GetLoginUrl(string redirectUri, string state)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ExternalUserInfo>> ExchangeCodeAsync(string code, string redirectUri)
    {
        throw new NotImplementedException();
    }
}