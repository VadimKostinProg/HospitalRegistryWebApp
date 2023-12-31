using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class CompleteAppointmentAsyncTests : AppointmentsServiceTestsBase
{
    private readonly Mock<HttpContext> _httpContextMock;

    public CompleteAppointmentAsyncTests()
    {
        _httpContextMock = new Mock<HttpContext>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        AppointmentCompleteRequest request = null;

        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Create<AppointmentCompleteRequest>();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(It.IsAny<Guid>(), false))
            .ReturnsAsync(null as Appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_AppointmentIsAlreadyCompleted_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var diagnosis = GetTestDiagnosis();
        var appointment = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(12, 0), new TimeOnly(14, 0)).First();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id, false))
            .ReturnsAsync(appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_AppointmentIsCanceled_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        appointment.Status = AppointmentStatus.Canceled.ToString();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id, false))
            .ReturnsAsync(appointment);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_IncorrectDiagnosisId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id, false))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(request.DiagnosisId, false))
            .ReturnsAsync(null as Diagnosis);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_DiagnosisIsDeleted_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();

        var diagnosis = GetTestDiagnosis();
        diagnosis.IsDeleted = true;

        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .With(x => x.DiagnosisId, diagnosis.Id)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id, false))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(diagnosis.Id, false))
            .ReturnsAsync(diagnosis);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_ConclusionIsNotPassed_ThrowsArgumentNullException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();

        var diagnosis = GetTestDiagnosis();

        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .With(x => x.DiagnosisId, diagnosis.Id)
            .With(x => x.Conclusion, string.Empty)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id, false))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(diagnosis.Id, false))
            .ReturnsAsync(diagnosis);

        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_InvalidDoctor_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var actualDoctor = GetTestDoctor();
        var patient = GetTestPatient();
        var diagnosis = GetTestDiagnosis();
        var appointment = GetTestScheduledAppointments(actualDoctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .With(x => x.DiagnosisId, diagnosis.Id)
            .Create();

        var claimsPrincipal = new ClaimsPrincipal();
        _httpContextMock.Setup(x => x.User)
            .Returns(claimsPrincipal);
        var httpContext = _httpContextMock.Object;
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContext);
        var user = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            DoctorId = doctor.Id
        };
        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id, false))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(diagnosis.Id, false))
            .ReturnsAsync(diagnosis);

        // Assert
        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CompleteAppointmentAsync_ValidObject_SuccessfullCompletingAppointment()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var diagnosis = GetTestDiagnosis();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .With(x => x.DiagnosisId, diagnosis.Id)
            .Create();

        var claimsPrincipal = new ClaimsPrincipal();
        _httpContextMock.Setup(x => x.User)
            .Returns(claimsPrincipal);
        var httpContext = _httpContextMock.Object;
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContext);
        var user = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            DoctorId = doctor.Id
        };
        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id, false))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(diagnosis.Id, false))
            .ReturnsAsync(diagnosis);

        // Assert
        // Assert
        var action = async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        };

        await action.Should().NotThrowAsync();
    }
}