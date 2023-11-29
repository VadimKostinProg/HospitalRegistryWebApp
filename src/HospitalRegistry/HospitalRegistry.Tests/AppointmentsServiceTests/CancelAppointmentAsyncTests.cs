using FluentAssertions;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class CancelAppointmentAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task CancelAppointmentAsyncTests_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(null as Appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CancelAppointmentAsyncTests_AppointmentIsAlreadyCanceled_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        appointment.Status = AppointmentStatus.Canceled.ToString();
        var idToPass = appointment.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CancelAppointmentAsyncTests_AppointmentIsCompleted_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var diagnosis = GetTestDiagnosis();
        var appointment = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(12, 0), new TimeOnly(15, 0)).First();
        var idToPass = appointment.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CancelAppointmentAsyncTests_ValidRequest_SuccessfullCancelingAppointment()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var idToPass = appointment.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        };

        await action.Should().NotThrowAsync();
    }
}
