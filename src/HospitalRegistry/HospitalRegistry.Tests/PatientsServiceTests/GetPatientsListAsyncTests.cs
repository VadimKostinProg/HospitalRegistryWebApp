using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetPatientsListAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task GetAllAsync_ReturnAllPatients()
    {
        // Arrange
        var patients = GetTestPatients().ToList();
        repositoryMock.Setup(x => x.GetAsync<Patient>(It.IsAny<ISpecification<Patient>>(), true))
                .ReturnsAsync(patients);
        PatientSpecificationsDTO? specifications = null;

        // Act
        var response = await service.GetPatientsListAsync(specifications);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(patients.Count(), response.Count());
    }
}