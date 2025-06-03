using System.Web;
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
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = _options.ClientId;
        query["redirect_uri"] = redirectUri;
        query["response_type"] = _options.ResponseType;
        query["scope"] = _options.Scope;
        query["state"] = state;
        
        return $"{_options.AuthorizationUrl}?{query}";
    }
    
    public Task<Result<ExternalUserInfo>> ExchangeCodeAsync(string code, string redirectUri)
    {
        throw new NotImplementedException();
    }
}