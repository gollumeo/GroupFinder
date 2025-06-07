using System.Collections.Concurrent;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.DTO;
using GroupFinder.Application.Common;

namespace GroupFinder.Persistence.Repositories;

public class InMemoryUsers : IUserRepository
{
    private readonly ConcurrentDictionary<string, RecognizedUser> _users = new();

    public Task<Result<RecognizedUser>> FindByExternalId(string externalId)
    {
        if (!_users.TryGetValue(externalId, out var user))
            return Task.FromResult(Result<RecognizedUser>.Failure("User not found."));

        return Task.FromResult(Result<RecognizedUser>.Success(user));
    }
}