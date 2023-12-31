using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class UpdateAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task UpdateAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        DiagnosisUpdateRequest request = null;

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }



    [Fact]
    public async Task UpdateAsync_InvalidName_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<DiagnosisUpdateRequest>()
            .With(x => x.Name, string.Empty)
            .Create();

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Create<DiagnosisUpdateRequest>();

        repositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>(), false))
            .ReturnsAsync(null as Diagnosis);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_NameAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var diagnosis = GetTestDiagnosis(isDeleted: true);

        var request = fixture.Create<DiagnosisUpdateRequest>();

        repositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>(), false))
            .ReturnsAsync(diagnosis);
        repositoryMock.Setup(x => x.ContainsAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();  
    }

    [Fact]
    public async Task UpdateAsync_ValidObject_SuccessfullUpdating()
    {
        // Arrange
        var diagnosis = GetTestDiagnosis();

        var request = fixture.Create<DiagnosisUpdateRequest>();

        repositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>(), false))
            .ReturnsAsync(diagnosis);
        repositoryMock.Setup(x => x.ContainsAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(false);
        repositoryMock.Setup(x => x.UpdateAsync<Diagnosis>(It.IsAny<Diagnosis>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await service.UpdateAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(request.Id);
        response.Name.Should().Be(request.Name);
    }
}