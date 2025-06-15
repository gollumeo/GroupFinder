using Application;
using GroupFinder.Application.Auth.ValueObjects;

namespace GroupFinder.Application.Auth.Contracts;

public interface IExternalAuthentication
{
    string GetLoginUrl(string redirectUri, string state);
    Task<Result<ExternalUserInfo>> ExchangeCodeAsync(string code, string redirectUri);
}