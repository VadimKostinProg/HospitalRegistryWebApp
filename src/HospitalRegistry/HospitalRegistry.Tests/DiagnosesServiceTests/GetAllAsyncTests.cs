using AutoFixture;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class GetAllAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllDiagnoses()
    {
        // Arrange
        var diagnoses = GetTestDiagnoses().AsQueryable();
        repositoryMock.Setup(x => x.GetAllAsync<Diagnosis>(true))
            .ReturnsAsync(diagnoses);

        // Act
        var actual = await service.GetAllAsync();
        
        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(diagnoses.Count(), actual.Count());
    }
}