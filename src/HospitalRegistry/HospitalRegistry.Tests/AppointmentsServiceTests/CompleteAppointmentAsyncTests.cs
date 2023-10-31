using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class CompleteAppointmentAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task CompleteAppointmentAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        AppointmentCompleteRequest request = null;
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        });
    }

    [Fact]
    public async Task CompleteAppointmentAsync_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Create<AppointmentCompleteRequest>();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(It.IsAny<Guid>()))
            .ReturnsAsync(null as Appointment);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        });
    }

    [Fact]
    public async Task CompleteAppointmentAsync_AppointmentIsAlreadyCompleted_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var diagnosis = GetDiagnosis();
        var appointment = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(12, 0), new TimeOnly(14, 0)).First();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id))
            .ReturnsAsync(appointment);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        });
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id))
            .ReturnsAsync(appointment);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        });
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

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(request.DiagnosisId))
            .ReturnsAsync(null as Diagnosis);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        });
    }

    [Fact]
    public async Task CompleteAppointmentAsync_ConclusionIsNotPassed_ThrowsArgumentNullException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .With(x => x.Conclusion, string.Empty)
            .Create();
        var diagnosis = GetDiagnosis();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(diagnosis.Id))
            .ReturnsAsync(diagnosis);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        });
    }

    [Fact]
    public async Task CompleteAppointmentAsync_ValidObject_SuccessfullCompletingAppointment()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var patient = GetTestPatient();
        var diagnosis = GetDiagnosis();
        var appointment = GetTestScheduledAppointments(doctor, patient, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(12, 0)).First();
        var request = fixture.Build<AppointmentCompleteRequest>()
            .With(x => x.Id, appointment.Id)
            .With(x => x.DiagnosisId, diagnosis.Id)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Appointment>(appointment.Id))
            .ReturnsAsync(appointment);
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(diagnosis.Id))
            .ReturnsAsync(diagnosis);
        
        // Assert
        Assert.Null(await Record.ExceptionAsync(async () =>
        {
            // Act
            await service.CompleteAppointmentAsync(request);
        }));
    }
}