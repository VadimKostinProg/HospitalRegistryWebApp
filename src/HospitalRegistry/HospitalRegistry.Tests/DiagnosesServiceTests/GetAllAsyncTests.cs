using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class GetAllAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllDiagnoses()
    {
        // Arrange
        var diagnoses = GetTestDiagnoses().ToList();
        var query = diagnoses.AsQueryable();
        repositoryMock.Setup(x => x.GetAllAsync<Diagnosis>(true))
            .ReturnsAsync(query);
        var specifications = new Specifications();
        specificationsServiceMock.Setup(x => x.ApplySpecifications(query, specifications))
            .Returns(query);

        // Act
        var actual = await service.GetAllAsync(specifications);
        
        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(diagnoses.Count(), actual.Count());
    }
}