using Application;
using GroupFinder.Application.Auth.DTO;
using GroupFinder.Application.Auth.ValueObjects;

namespace GroupFinder.Application.Auth.Contracts;

public interface IExternalUserRecognition
{
    Task<Result<RecognizedUser>> From(ExternalUserInfo info);
}