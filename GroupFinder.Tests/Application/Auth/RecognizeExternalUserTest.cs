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

    [Fact]
    public async Task RelaysFailureFromRecognitionGateway()
    {
        var externalInfo = new ExternalUserInfo("foo", "bar", "eu");

        var gateway = new Mock<IExternalUserRecognition>();
        gateway.Setup(x => x.From(externalInfo))
            .ReturnsAsync(Result<RecognizedUser>.Failure("User not found."));

        var useCase = new RecognizeExternalUser(gateway.Object);
        var result = await useCase.Execute(externalInfo);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found.");
    }
}