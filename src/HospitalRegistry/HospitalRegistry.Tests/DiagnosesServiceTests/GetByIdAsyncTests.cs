using AutoFixture;
using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class GetByIdAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Attange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>(), true))
            .ReturnsAsync(null as Diagnosis);
        
        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetByIdAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsObject()
    {
        // Arrange
        var diagnosis = GetTestDiagnosis();
        var idToPass = diagnosis.Id;
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>(), true))
            .ReturnsAsync(diagnosis);
        
        // Act
        var response = await service.GetByIdAsync(idToPass);
        
        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(diagnosis.Id);
        response.Name.Should().Be(diagnosis.Name);
    }
}