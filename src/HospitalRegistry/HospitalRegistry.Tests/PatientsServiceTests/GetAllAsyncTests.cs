using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
        repositoryMock.Setup(x => x.GetAsync<Patient>(true))
                .ReturnsAsync(query);
        Specifications? specifications = null;

        // Act
        var response = await service.GetAllAsync(specifications);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(patients.Count(), response.Count());
    }
}