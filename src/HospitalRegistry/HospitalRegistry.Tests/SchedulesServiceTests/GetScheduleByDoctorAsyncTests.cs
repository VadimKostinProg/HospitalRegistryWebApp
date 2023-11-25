using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.SchedulesServiceTests;

public class GetScheduleByDoctorAsyncTests : SchedulesServiceTestsBase
{
    [Fact]
    public async Task GetScheduleByDoctorAsync_IncorrectDoctorsId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass, true))
            .ReturnsAsync(null as Doctor);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            var schedule = await service.GetScheduleByDoctorAsync(idToPass);
        });
    }

    [Fact]
    public async Task GetScheduleByDoctorAsync_ValidDoctorsId_RetunsDoctorsSchedule()
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
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(doctorId, true))
            .ReturnsAsync(doctor);
        
        // Act
        var schedule = await service.GetScheduleByDoctorAsync(doctorId);
        
        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(doctorId, schedule.DoctorId);
        Assert.Equal(testSchedule.Count, schedule.Schedule.Count);
    }

    [Fact]
    public async Task GetScheduleByDoctorAsync_DayOfWeekPassed_ReturnsScheduleForThatDayOfWeek()
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
        var dayOfWeek = 1;
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(doctorId, true))
            .ReturnsAsync(doctor);
        
        // Act
        var schedule = await service.GetScheduleByDoctorAsync(doctorId, dayOfWeek);
        
        // Assert
        Assert.NotNull(schedule);
        Assert.True(schedule.Schedule.All(x => x.DayOfWeek == dayOfWeek));
    }
}