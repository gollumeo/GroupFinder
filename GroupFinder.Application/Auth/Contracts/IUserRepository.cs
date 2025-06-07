using GroupFinder.Application.Auth.DTO;
using GroupFinder.Application.Common;

namespace GroupFinder.Application.Auth.Contracts;

public interface IUserRepository
{
    Task<Result<RecognizedUser>> FindByExternalId(string externalId);
}