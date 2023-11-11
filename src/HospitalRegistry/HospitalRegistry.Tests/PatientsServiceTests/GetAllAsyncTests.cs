using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetAllAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task GetAllAsync_ReturnAllPatients()
    {
        // Arrange
        var patients = GetTestPatients().ToList();
        var query = patients.AsQueryable();
        repositoryMock.Setup(x => x.GetFilteredAsync<Patient>(It.IsAny<Expression<Func<Patient, bool>>>(), true))
                .ReturnsAsync(query);
        var specifications = new Specifications();
        specificationsServiceMock.Setup(x => x.ApplySpecifications(query, specifications))
            .Returns(query);

        // Act
        var response = await service.GetAllAsync(specifications);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(patients.Count(), response.Count());
    }
}