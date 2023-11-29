using AutoFixture;
using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class GetAppointmentsHistoryOfPatientAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task GetAppointmentsHistoryOfPatientAsync_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(null as Patient);

        // Assert
        var action = async () =>
        {
            // Act
            var history = await service.GetAppointmentsHistoryOfPatientAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetAppointmentsHistoryOfPatientAsync_ValidId_ReturnsAppointmentsHistory()
    {
        // Arrange
        var patient = GetTestPatient();
        var doctor = GetTestDoctor();
        var diagnosis = new Diagnosis 
        { 
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>(),
        };

        var appointments = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(12, 0), new TimeOnly(14, 0));
        patient.Appointments = appointments.ToList();

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(patient.Id, true))
            .ReturnsAsync(patient);

        // Act
        var history = await service.GetAppointmentsHistoryOfPatientAsync(patient.Id);

        // Assert
        history.Should().NotBeNull();
        history.Should().NotBeEmpty();
        history.Should().OnlyContain(x => x.Patient.Id == patient.Id);
    }
}