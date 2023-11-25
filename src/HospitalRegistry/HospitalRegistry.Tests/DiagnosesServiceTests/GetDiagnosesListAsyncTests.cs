using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class GetDiagnosesListAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllDiagnoses()
    {
        // Arrange
        var diagnoses = GetTestDiagnoses().ToList();
        repositoryMock.Setup(x => x.GetAsync<Diagnosis>(It.IsAny<ISpecification<Diagnosis>>(), true))
            .ReturnsAsync(diagnoses);
        DiagnosisSpecificationsDTO? specifications = null;

        // Act
        var actual = await service.GetDiagnosesListAsync(specifications);
        
        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(diagnoses.Count(), actual.Count());
    }
}