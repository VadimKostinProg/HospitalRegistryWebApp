using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class RecoverAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task RecoverAsyncTests_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(null as Appointment);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        });
    }

    [Fact]
    public async Task RecoverAsyncTests_AppointmentIsNotCanceled_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var idToPass = appointment.Id;

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(appointment);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        });
    }

    [Fact]
    public async Task RecoverAsyncTests_AppointmentIsTryedToBeRecoveredAfterMaxLateness_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var idToPass = appointment.Id;

        int delay = AppointmentsConfiguration.MaxAppointmentRecoveringLateness + 1;

        appointment.Status = AppointmentStatus.Canceled.ToString();
        appointment.DateAndTime = DateTime.UtcNow.AddMinutes(-delay);

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(appointment);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        });
    }

    [Fact]
    public async Task RecoverAsyncTests_AlreadyAnotherAppointmentOnThisTimeExists_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var idToPass = appointment.Id;

        appointment.Status = AppointmentStatus.Canceled.ToString();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(true);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        });
    }

    [Fact]
    public async Task RecoverAsyncTests_ValidRequest_SuccessfullReoveringAppointment()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var idToPass = appointment.Id;

        appointment.Status = AppointmentStatus.Canceled.ToString();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, false))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(false);

        // Assert
        Assert.Null(await Record.ExceptionAsync(async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        }));
    }
}