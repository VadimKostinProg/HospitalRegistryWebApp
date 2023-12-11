using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetByIdAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Patient>(It.IsAny<Expression<Func<Patient, bool>>>(), false))
            .ReturnsAsync(null as Patient);
        
        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetByIdAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsObject()
    {
        // Arrange
        var patient = GetTestPatient();
        var idToPass = patient.Id;
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Patient>(It.IsAny<Expression<Func<Patient, bool>>>(), false))
            .ReturnsAsync(patient);
        
        // Act
        var response = await service.GetByIdAsync(idToPass);
        
        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(idToPass);
        response.Name.Should().Be(patient.Name);
        response.Surname.Should().Be(patient.Surname);
        response.Patronymic.Should().Be(patient.Patronymic);
        response.Email.Should().Be(patient.Email);
        response.PhoneNumber.Should().Be(patient.PhoneNumber);
    }
}