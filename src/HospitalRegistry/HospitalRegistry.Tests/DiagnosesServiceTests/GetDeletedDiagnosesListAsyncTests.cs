using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;
public class GetDeletedDiagnosesListAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task GetDeletedDiagnosesListAsync_ReturnsAllDiagnoses()
    {
        // Arrange
        var diagnoses = GetTestDiagnoses(isDeleted: true).ToList();
        repositoryMock.Setup(x => x.GetAsync<Diagnosis>(It.IsAny<ISpecification<Diagnosis>>(), true))
            .ReturnsAsync(diagnoses);
        var specifications = fixture.Create<DiagnosisSpecificationsDTO>();

        // Act
        var actual = await service.GetDeletedDiagnosesListAsync(specifications);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(diagnoses.Count(), actual.Count());
    }
}