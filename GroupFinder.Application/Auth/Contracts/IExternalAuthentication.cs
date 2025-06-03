using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;

namespace GroupFinder.Application.Auth.Contracts;

public interface IExternalAuthentication
{
    string GetLoginUrl(string redirectUri, string state);
    Task<Result<ExternalUserInfo>> ExchangeCodeAsync(string code, string redirectUri);
}