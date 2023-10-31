using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetAllAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task GetAllAsync_ReturnAllPatients()
    {
        // Arrange
        var patients = GetTestPatients();
        repositoryMock.Setup(x => x.GetFilteredAsync<Patient>(It.IsAny<Expression<Func<Patient, bool>>>(), true))
                .ReturnsAsync(patients);

        // Act
        var response = await service.GetAllAsync();
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(patients.Count(), response.Count());
    }
}