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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), false))
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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), false))
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

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), false))
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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), false))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), false))
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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), false))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), false))
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

        var tomorrowDate = DateTime.UtcNow.AddDays(1);

        var dateTimeToSet = new DateTime(tomorrowDate.Year, tomorrowDate.Month, tomorrowDate.Day, 10, 0, 0);

        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, dateTimeToSet)
            .With(x => x.DoctorId, doctor.Id)
            .With(x => x.PatientId, patient.Id)
            .Create();

        var schedules = GetTestSchedules(doctor.Id, dayOfWeek: (int)dateTimeToSet.DayOfWeek);
        
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>(), false))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>(), false))
            .ReturnsAsync(patient);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(false);
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Schedule>(It.IsAny<Expression<Func<Schedule, bool>>>(), false))
            .ReturnsAsync(schedules.First());

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        };

        await action.Should().NotThrowAsync();
    }
}