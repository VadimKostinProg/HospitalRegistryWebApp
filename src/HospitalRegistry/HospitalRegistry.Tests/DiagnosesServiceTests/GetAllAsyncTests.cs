using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
        repositoryMock.Setup(x => x.GetAsync<Diagnosis>(true))
            .ReturnsAsync(query);
        Specifications? specifications = null;

        // Act
        var actual = await service.GetAllAsync(specifications);
        
        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(diagnoses.Count(), actual.Count());
    }
}