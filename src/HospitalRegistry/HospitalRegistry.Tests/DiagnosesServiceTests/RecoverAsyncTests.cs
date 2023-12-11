using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class RecoverAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task RecoverAsync_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(idToPass, false))
            .ReturnsAsync(null as Diagnosis);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };
        
        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task RecoverAsync_DiagnosisIsNotDeleted_ThrowsArgumentException()
    {
        // Arrange
        var diagnosis = GetTestDiagnosis();

        var idToPass = diagnosis.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(idToPass, false))
            .ReturnsAsync(diagnosis);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task RecoverAsync_DiagnosisIsNotDeleted_ThrowsInvalidOperationException()
    {
        // Arrange
        var diagnosis = GetTestDiagnosis(isDeleted: true);

        var idToPass = diagnosis.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(idToPass, false))
           .ReturnsAsync(diagnosis);
        repositoryMock.Setup(x => x.ContainsAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task RecoverAsync_ValidRequest_SuccessfullRecovering()
    {
        // Arrange
        var diagnosis = GetTestDiagnosis(isDeleted: true);

        var idToPass = diagnosis.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(idToPass, false))
           .ReturnsAsync(diagnosis);
        repositoryMock.Setup(x => x.ContainsAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(false);
        repositoryMock.Setup(x => x.UpdateAsync<Diagnosis>(It.IsAny<Diagnosis>()))
            .Returns(Task.CompletedTask);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };

        await action.Should().NotThrowAsync();
    }
}
