using FluentAssertions;
using GroupFinder.Application;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Persistence.Repositories;

namespace GroupFinder.Tests.Application.Auth;

public class AuthenticateUserViaSsoTest
{
    [Fact]
    public async Task CreatesANewUserOnFirstLogin()
    {
        // Given
        var users = new InMemoryUsers();
        var useCase = new AuthenticateUserViaSso(users);

        var info = new ExternalUserInfo(
            "bnet-123456",
            "Thrall#1234",
            "eu"
        );

        // When
        var result = await useCase.Execute(info);

        // Then
        result.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();

        // TODO: handle dictionary populating strategy
        var user = await users.FindByExternalId("bnet-123456");

        user.Should().NotBeNull();
        user.Value.BattleTag.Should().Be("Thrall#1234");
        user.Value.Region.Should().Be("eu");
    }
}