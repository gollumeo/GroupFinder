using Application;
using GroupFinder.Application.Auth.DTO;

namespace GroupFinder.Application.Auth.Contracts;

public interface IUserRepository
{
    Task<Result<RecognizedUser>> FindByExternalId(string externalId);
    Task Add(RecognizedUser user);
}