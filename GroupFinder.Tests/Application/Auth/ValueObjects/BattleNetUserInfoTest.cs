using FluentAssertions;
using GroupFinder.Application.Auth.ValueObjects;

namespace GroupFinder.Tests.Application.Auth.ValueObjects;

public class BattleNetUserInfoTest
{
    [Fact]
    public void CreatesSucceedsWithValidInputs()
    {
        const string id = "bnet-123456";
        const string tag = "Thrall#1234";
        const string region = "eu";
        
        var result = BattleNetUserInfo.Create(id, tag, region);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.BattleTag.Should().Be(tag);
        result.Value.Region.Should().Be(region);
    }
}