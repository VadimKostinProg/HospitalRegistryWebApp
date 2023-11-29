using System.Linq.Expressions;
using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class GetScheduledAppoitnmentsOfPatientAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task GetScheduledAppoitnmentsOfPatient_InvalidPatientId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(null as Patient);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfPatientAsync(idToPass, date);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfPatient_PatientIsDeleted_ThrowsArgumentException()
    {
        // Arrange
        var patient = GetTestPatient();
        var idToPass = patient.Id;
        patient.IsDeleted = true;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(patient);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfPatientAsync(idToPass, date);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfPatient_DateLassThanToday_ThrowsArgumentException()
    {
        // Arrange
        var patient = GetTestPatient();
        var idToPass = patient.Id;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(patient);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfPatientAsync(idToPass, date);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfPatient_ValidParams_RetunsScheduledAppointments()
    {
        // Arrange
        var patient = GetTestPatient();
        var idToPass = patient.Id;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));
        var doctor = GetTestDoctor();
        var appointments = GetTestScheduledAppointments(doctor, patient, date, new TimeOnly(12, 0)).AsQueryable();

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(patient);
        repositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), true))
            .ReturnsAsync(appointments);

        // Act
        var response = await service.GetScheduledAppoitnmentsOfPatientAsync(idToPass, date);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().NotBeEmpty();
        response.Should().OnlyContain(x => x.Patient.Id == patient.Id &&
                                      DateOnly.FromDateTime(x.DateAndTime) == date);
    }
}