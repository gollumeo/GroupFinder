using GroupFinder.Application.Auth.DTO;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;

namespace GroupFinder.Application.Auth.Contracts;

public interface IExternalUserRecognition
{
    Task<Result<RecognizedUser>> From(ExternalUserInfo info);
}