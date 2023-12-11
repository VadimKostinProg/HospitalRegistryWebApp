using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.SchedulesServiceTests;

public class SetAsyncTests : SchedulesServiceTestsBase
{
    [Fact]
    public async Task SetAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        ScheduleDTO schedule = null;
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            await service.SetAsync(schedule);
        });
    }
    
    [Fact]
    public async Task SetAsync_InvalidDoctorsId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var testSchedule = GetTestSchedules(doctorId)
            .Select(x => x.ToTimeSlotDTO())
            .ToList();
        var scheduleSetRequest = fixture.Build<ScheduleDTO>()
            .With(x => x.DoctorId, doctorId)
            .With(x => x.Schedule, testSchedule)
            .Create();
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), false))
            .ReturnsAsync(null as Doctor);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.SetAsync(scheduleSetRequest);
        });
    }

    [Fact]
    public async Task SetAsync_EmptyScheduleCollection_ThrowsArgumentException()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var testSchedule = GetTestSchedules(doctorId).ToList();
        var doctor = fixture.Build<Doctor>()
            .With(x => x.Id, doctorId)
            .With(x => x.Schedules, testSchedule)
            .With(x => x.DateOfBirth, "01.01.2000")
            .With(x => x.Appointments, new List<Appointment>())
            .Create();
        var newSchedule = new List<TimeSlotDTO>();
        var scheduleSetRequest = fixture.Build<ScheduleDTO>()
            .With(x => x.DoctorId, doctorId)
            .With(x => x.Schedule, newSchedule)
            .Create();
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), false))
            .ReturnsAsync(doctor);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAsync(scheduleSetRequest);
        };
        
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task SetAsync_TimeSlotsCross_ThrowsArgumentException()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var testSchedule = GetTestSchedules(doctorId).ToList();
        var doctor = fixture.Build<Doctor>()
            .With(x => x.Id, doctorId)
            .With(x => x.Schedules, testSchedule)
            .With(x => x.DateOfBirth, "01.01.2000")
            .With(x => x.Appointments, new List<Appointment>())
            .Create();
        var newSchedule = GetTestCrossedTimeSlotsDTO().ToList();
        var scheduleSetRequest = fixture.Build<ScheduleDTO>()
            .With(x => x.DoctorId, doctorId)
            .With(x => x.Schedule, newSchedule)
            .Create();
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), false))
            .ReturnsAsync(doctor);

        // Assert
        var action = async () =>
        {
            // Act
            await service.SetAsync(scheduleSetRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SetAsync_SetValidTimeSlots_SuccessfullSettingSchedule()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var testSchedule = GetTestSchedules(doctorId).ToList();
        var doctor = fixture.Build<Doctor>()
            .With(x => x.Id, doctorId)
            .With(x => x.Schedules, testSchedule)
            .With(x => x.DateOfBirth, "01.01.2000")
            .With(x => x.Appointments, new List<Appointment>())
            .Create();
        var newSchedule = testSchedule.Take(3).Select(x => x.ToTimeSlotDTO()).ToList();
        var scheduleSetRequest = fixture.Build<ScheduleDTO>()
            .With(x => x.DoctorId, doctorId)
            .With(x => x.Schedule, newSchedule)
            .Create();
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), false))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.DeleteRangeAsync(testSchedule))
            .ReturnsAsync(testSchedule.Count());
        repositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<TimeSlot, bool>>>(), false))
            .ReturnsAsync(null as TimeSlot);
        repositoryMock.Setup(x => x.AddAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);
        repositoryMock.Setup(x => x.AddAsync(It.IsAny<TimeSlot>()))
            .Returns(Task.CompletedTask);

        var action = async () =>
        {
            // Act
            await service.SetAsync(scheduleSetRequest);
        };

        await action.Should().NotThrowAsync();
    }
}