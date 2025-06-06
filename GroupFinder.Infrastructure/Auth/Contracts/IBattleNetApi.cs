using GroupFinder.Infrastructure.Auth.Models;

namespace GroupFinder.Infrastructure.Auth.Contracts;

public interface IBattleNetApi
{
    Task<BattleNetTokenResponse> ExchangeCodeForToken(string code, string redirectUri);
    Task<BattleNetUserInfoDto> FetchUserInfo(string accessToken);
}
