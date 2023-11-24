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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(null as Appointment);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        });
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(appointment);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        });
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(appointment);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        });
    }

    [Fact]
    public async Task CancelAppointmentAsyncTests_ValidRequest_SuccessfullCancelingAppointment()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var idToPass = appointment.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(appointment);

        // Assert
        Assert.Null(await Record.ExceptionAsync(async () =>
        {
            // Act
            await service.CancelAppointmentAsync(idToPass);
        }));
    }
}
