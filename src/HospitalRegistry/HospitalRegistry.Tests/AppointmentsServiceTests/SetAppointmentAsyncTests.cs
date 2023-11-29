using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class SetAppointmentAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task SetAppointmentAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        AppointmentSetRequest request = null;
        
        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SetAppointmentAsync_DateAndTimeLessThanNow_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(-2))
            .Create();

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task SetAppointmentAsync_IncorrectDoctorId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), true))
            .ReturnsAsync(null as Doctor);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
    
    [Fact]
    public async Task SetAppointmentAsync_DoctorIsDeleted_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .With(x => x.DoctorId, doctor.Id)
            .Create();
        doctor.IsDeleted = true;

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), true))
            .ReturnsAsync(doctor);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task SetAppointmentAsync_IncorrectPatientId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), true))
            .ReturnsAsync(null as Patient);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
    
    [Fact]
    public async Task SetAppointmentAsync_PatientIsDeleted_ThrowsArgumentException()
    {
        // Arrange
        var patient = GetTestPatient();
        var doctor = GetTestDoctor();
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .With(x => x.DoctorId, doctor.Id)
            .With(x => x.PatientId, patient.Id)
            .Create();
        patient.IsDeleted = true;

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), true))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), true))
            .ReturnsAsync(patient);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SetAppointmnetAsync_AnotherAppointmnetHasBeenSetOnThisTime_ThrowsArgumentException()
    {
        // Arrange
        var patient = GetTestPatient();
        var doctor = GetTestDoctor();
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .With(x => x.DoctorId, doctor.Id)
            .With(x => x.PatientId, patient.Id)
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), true))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), true))
            .ReturnsAsync(patient);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SetAppointmentAsync_ValidObject_SuccessfullSettingAppointment()
    {
        // Arrange
        var patient = GetTestPatient();
        var doctor = GetTestDoctor();
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .With(x => x.DoctorId, doctor.Id)
            .With(x => x.PatientId, patient.Id)
            .Create();
        
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), true))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), true))
            .ReturnsAsync(patient);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(false);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().NotThrowAsync();
    }
}