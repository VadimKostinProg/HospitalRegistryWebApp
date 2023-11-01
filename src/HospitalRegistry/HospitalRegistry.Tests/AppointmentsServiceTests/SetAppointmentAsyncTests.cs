using AutoFixture;
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
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        });
    }

    [Fact]
    public async Task SetAppointmentAsync_DateAndTimeLessThanNow_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(-2))
            .Create();
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        });
    }
    
    [Fact]
    public async Task SetAppointmentAsync_IncorrectDoctorId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>()))
            .ReturnsAsync(null as Doctor);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        });
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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>()))
            .ReturnsAsync(doctor);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        });
    }
    
    [Fact]
    public async Task SetAppointmentAsync_IncorrectPatientId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Build<AppointmentSetRequest>()
            .With(x => x.DateAndTime, DateTime.UtcNow.AddDays(2))
            .Create();

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>()))
            .ReturnsAsync(null as Patient);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        });
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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>()))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>()))
            .ReturnsAsync(patient);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        });
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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>()))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>()))
            .ReturnsAsync(patient);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(true);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        });
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
        
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(It.IsAny<Guid>()))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(It.IsAny<Guid>()))
            .ReturnsAsync(patient);
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(false);

        // Assert
        Assert.Null(await Record.ExceptionAsync(async () =>
        {
            // Act
            await service.SetAppointmentAsync(request);
        }));
    }
}