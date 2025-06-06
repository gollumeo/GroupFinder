using FluentAssertions;
using GroupFinder.Infrastructure.Auth.ValueObjects;

namespace GroupFinder.Tests.Application.Auth.ValueObjects;

public class BattleNetUserInfoTest
{
    [Fact]
    public void CreateSucceedsWithValidInputs()
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void CreateFailsWithNullOrEmptyId(string? invalidId)
    {
        const string tag = "Thrall#1234";
        const string region = "eu";

        var result = BattleNetUserInfo.Create(invalidId!, tag, region);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("id");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void CreateFailsWithNullOrEmptyBattleTag(string? invalidTag)
    {
        const string id = "bnet-123456";
        const string region = "eu";

        var result = BattleNetUserInfo.Create(id, invalidTag!, region);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("battleTag");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("kr")]
    [InlineData("na")]
    [InlineData("")]
    [InlineData("     ")]
    public void CreateFailsWithNullOrEmptyRegion(string? invalidRegion)
    {
        const string id = "bnet-123456";
        const string tag = "Thrall#1234";

        var result = BattleNetUserInfo.Create(id, tag, invalidRegion!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("region");
    }
}