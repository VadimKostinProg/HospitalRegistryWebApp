using System.Linq.Expressions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using Moq;
using It = Moq.It;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class SearchFreeSlotsAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task SearchFreeSlotsAsync_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = null;
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }

    [Fact]
    public async Task SearchFreeSlotsAsync_StartDateLessTheCurrentDate_ThrowsArgumentException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            AppointmentType = AppointmentType.Consultation,
            Specialty = Specialty.Allergist
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }

    [Fact]
    public async Task SearchFreeSlotsAsync_StartDateMoreThanEndDate_ThrowsArgumentException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            AppointmentType = AppointmentType.Consultation,
            Specialty = Specialty.Allergist
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }
    
    [Fact]
    public async Task SearchFreeSlotsAsync_DoctorIdAndSpecialtyAreBothNull_ThrowsArgumentNullException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            AppointmentType = AppointmentType.Consultation
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }

    [Fact]
    public async Task SearchFreeSlotsAsync_DoctorIdIsNotPassed_ReturnsFreeSlotsBySpecialty()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            AppointmentType = AppointmentType.Consultation,
            Specialty = Specialty.Allergist
        };

        var doctors = GetTestDoctors().Select(x => x.ToDoctorResponse());

        doctorsServiceMock.Setup(x => x.GetBySpecialtyAsync(It.IsAny<Specialty>()))
            .ReturnsAsync(doctors);
        schedulesServiceMock.Setup(x => x.GetScheduleByDoctorAsync(It.IsAny<Guid>(), null))
            .ReturnsAsync(GetTestScheduleDTO(It.IsAny<Guid>()));
        repositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), true))
            .ReturnsAsync(new List<Appointment>());
        
        // Act
        var response = await service.SearchFreeSlotsAsync(specifications);
        
        // Assert
        Assert.NotNull(response);
        Assert.True(response.All(x => x.Specialty == specifications.Specialty));
        Assert.True(response.Min(x => x.Date) >= specifications.StartDate);
        Assert.True(response.Max(x => x.Date) <= specifications.EndDate);
    }
    
    [Fact]
    public async Task SearchFreeSlotsAsync_DoctorIdIsPassed_ReturnsFreeSlotsByDoctor()
    {
        // Arrange
        var doctor = GetTestDoctor().ToDoctorResponse();
        
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            AppointmentType = AppointmentType.Consultation,
            DoctorId = doctor.Id
        };

        doctorsServiceMock.Setup(x => x.GetByIdAsync(doctor.Id))
            .ReturnsAsync(doctor);
        schedulesServiceMock.Setup(x => x.GetScheduleByDoctorAsync(doctor.Id, null))
            .ReturnsAsync(GetTestScheduleDTO(doctor.Id));
        repositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), true))
            .ReturnsAsync(new List<Appointment>());
        
        // Act
        var response = await service.SearchFreeSlotsAsync(specifications);
        
        // Assert
        Assert.NotNull(response);
        Assert.True(response.All(x => x.DoctorId == specifications.DoctorId));
        Assert.True(response.Min(x => x.Date) >= specifications.StartDate);
        Assert.True(response.Max(x => x.Date) <= specifications.EndDate);
    }
    
    [Fact]
    public async Task SearchFreeSlotsAsync_DoctorIdIsPassedAndAppointmentsAlreadyExists_ReturnsFreeSlotsByDoctor()
    {
        // Arrange
        var doctor = GetTestDoctor().ToDoctorResponse();
        
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            AppointmentType = AppointmentType.Consultation,
            DoctorId = doctor.Id
        };

        doctorsServiceMock.Setup(x => x.GetByIdAsync(doctor.Id))
            .ReturnsAsync(doctor);
        schedulesServiceMock.Setup(x => x.GetScheduleByDoctorAsync(doctor.Id, null))
            .ReturnsAsync(GetTestScheduleDTO(doctor.Id));
        repositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), true))
            .ReturnsAsync(GetTestScheduledAppointments(doctor.Id, Guid.NewGuid(), specifications.StartDate, new TimeOnly(12, 00)));
        
        // Act
        var response = await service.SearchFreeSlotsAsync(specifications);
        
        // Assert
        Assert.NotNull(response);
        Assert.True(response.All(x => x.DoctorId == specifications.DoctorId));
        Assert.True(response.Min(x => x.Date) >= specifications.StartDate);
        Assert.True(response.Max(x => x.Date) <= specifications.EndDate);
    }
}