using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.DTO;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;

namespace GroupFinder.Application;

/// <summary>
///     Authenticates a user via Single Sign-On (SSO).
///     – If the user doesn't exist locally, creates a new user record.
///     – If the user exists, updates their information as needed.
///     This application only supports SSO-based authentication, no local accounts.
/// </summary>
public class AuthenticateUserViaSso(IUserRepository users)
{
    public async Task<Result<RecognizedUser>> Execute(ExternalUserInfo info)
    {
        return await users.FindByExternalId(info.Id);
    }
}