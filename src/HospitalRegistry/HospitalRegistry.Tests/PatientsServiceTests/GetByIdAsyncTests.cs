using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetByIdAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(null as Patient);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            var response = await service.GetByIdAsync(idToPass);
        });
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsObject()
    {
        // Arrange
        var patient = GetTestPatient();
        var idToPass = patient.Id;
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(patient);
        
        // Act
        var response = await service.GetByIdAsync(idToPass);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(idToPass, response.Id);
        Assert.Equal(patient.Name, response.Name);
        Assert.Equal(patient.Surname, response.Surname);
        Assert.Equal(patient.Patronymic, response.Patronymic);
        Assert.Equal(patient.Name, response.Name);
        Assert.Equal(patient.Name, response.Name);
    }
}