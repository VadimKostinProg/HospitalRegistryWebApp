using FluentAssertions;
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(null as Appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task RecoverAsyncTests_AppointmentIsNotCanceled_ThrowsArgumentException()
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
            await service.RecoverAsync(idToPass);
        };

        await action.Should().ThrowAsync<ArgumentException>();
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };

        await action.Should().ThrowAsync<ArgumentException>();
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };

        await action.Should().ThrowAsync<ArgumentException>();
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(idToPass, true))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(false);

        // Assert
        var action = async () =>
        {
            // Act
            await service.RecoverAsync(idToPass);
        };

        await action.Should().NotThrowAsync();
    }
}