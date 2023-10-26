using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetAllAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task GetAllAsync_ReturnAllPatients()
    {
        // Arrange
        var patients = GetTestPatients();
        repositoryMock.Setup(x => x.GetAllAsync<Patient>(true))
            .ReturnsAsync(patients);
        
        // Act
        var response = await service.GetAllAsync();
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(patients.Count(), response.Count());
    }
}