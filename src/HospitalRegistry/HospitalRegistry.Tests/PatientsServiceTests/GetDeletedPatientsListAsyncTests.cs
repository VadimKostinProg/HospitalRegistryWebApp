using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetDeletedPatientsListAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task GetDeletedPatientsListAsync_ReturnAllPatients()
    {
        // Arrange
        var patients = GetTestPatients(isDeleted: true).ToList();
        repositoryMock.Setup(x => x.GetAsync<Patient>(It.IsAny<ISpecification<Patient>>(), true))
                .ReturnsAsync(patients);
        var specifications = fixture.Build<PatientSpecificationsDTO>()
            .With(x => x.DateOfBirth, DateOnly.Parse("01.01.2000"))
            .Create();

        // Act
        var response = await service.GetPatientsListAsync(specifications);

        // Assert
        response.Should().NotBeNull();
        response.Count().Should().Be(patients.Count());
    }
}
