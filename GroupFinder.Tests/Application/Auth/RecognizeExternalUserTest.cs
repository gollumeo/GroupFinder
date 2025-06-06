using FluentAssertions;
using GroupFinder.Application.Auth;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Application.Auth.DTO;
using GroupFinder.Application.Auth.ValueObjects;
using GroupFinder.Application.Common;
using Moq;

namespace GroupFinder.Tests.Application.Auth;

public class RecognizeExternalUserTest
{
    [Fact]
    public async Task ReturnsRecognizedExternalUser()
    {
        var externalInfo = new ExternalUserInfo("ext-123", "DarkDk#0666", "eu");
        
        var fakeUserId = Guid.NewGuid();

        var gateway = new Mock<IExternalUserRecognition>();
        gateway.Setup(x => x.From(externalInfo))
            .ReturnsAsync(Result<RecognizedUser>.Success(new RecognizedUser(fakeUserId)));

        var useCase = new RecognizeExternalUser(gateway.Object);
        var result = await useCase.Execute(externalInfo);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(fakeUserId);
    }
    
    [Fact]
    public async Task ReturnsFailureWhenExternalUserNotFound()
    {
        await Task.CompletedTask;
    }

    [Theory]
    [InlineData("id-123", "Waza#1234", "na")]
    [InlineData("", "Waza#1234", "eu")]
    [InlineData("", "", "eu")]
    [InlineData("", "", "")]
    public async Task ReturnsEarlyWithInvalidInput(string id, string battleTag, string region)
    {
        var externalInfo = new ExternalUserInfo(id, battleTag, region);
        
        var recognition = new Mock<IExternalUserRecognition>();
        recognition.Setup(x => x.From(It.IsAny<ExternalUserInfo>()))
            .ReturnsAsync(Result<RecognizedUser>.Failure("Invalid external user info."));
        
        var useCase = new RecognizeExternalUser(recognition.Object);
        var result = await useCase.Execute(externalInfo);
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid external user info.");
        
        recognition.Verify(x => x.From(It.IsAny<ExternalUserInfo>()), Times.Never);
    }
}