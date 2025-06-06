using System.Web;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;
using GroupFinder.Infrastructure.Auth.Contracts;
using GroupFinder.Infrastructure.Auth.Options;
using GroupFinder.Infrastructure.Auth.ValueObjects;
using Microsoft.Extensions.Options;

namespace GroupFinder.Infrastructure.Auth;

public class BattleNetAuthProxy(
    IBattleNetApi api,
    IOptions<BattleNetOAuthOptions> options
) : IExternalAuthentication
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

    public async Task<Result<ExternalUserInfo>> ExchangeCodeAsync(string code, string redirectUri)
    {
        try
        {
            var token = await api.ExchangeCodeForToken(code, redirectUri);
            var userDto = await api.FetchUserInfo(token.AccessToken);

            var battleNetUserInfo = BattleNetUserInfo.Create(userDto.Id, userDto.BattleTag, "eu");
            if (battleNetUserInfo.IsFailure)
                return Result<ExternalUserInfo>.Failure(battleNetUserInfo.Error);

            return Result<ExternalUserInfo>.Success(new ExternalUserInfo(
                battleNetUserInfo.Value.Id,
                battleNetUserInfo.Value.BattleTag,
                battleNetUserInfo.Value.Region
            ));
        }
        catch (Exception ex)
        {
            return Result<ExternalUserInfo>.Failure($"Exception during token exchange: {ex.Message}");
        }
    }
}