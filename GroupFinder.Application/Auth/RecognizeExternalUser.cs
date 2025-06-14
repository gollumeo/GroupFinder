﻿using Application;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.ValueObjects;

namespace GroupFinder.Application.Auth;

public class RecognizeExternalUser(IExternalUserRecognition recognition)
{
    public async Task<Result<Guid>> Execute(ExternalUserInfo info)
    {
        var result = await recognition.From(info);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error);

        return Result<Guid>.Success(result.Value.Id);
    }
}