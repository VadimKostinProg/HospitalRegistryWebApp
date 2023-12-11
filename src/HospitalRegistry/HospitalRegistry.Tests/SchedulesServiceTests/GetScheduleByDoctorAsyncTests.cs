using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.SchedulesServiceTests;

public class GetScheduleByDoctorAsyncTests : SchedulesServiceTestsBase
{
    [Fact]
    public async Task GetScheduleByDoctorAsync_IncorrectDoctorsId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), true))
            .ReturnsAsync(null as Doctor);
        
        // Assert
        var action = async () =>
        {
            // Act
            var schedule = await service.GetScheduleByDoctorAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
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
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), true))
            .ReturnsAsync(doctor);
        
        // Act
        var schedule = await service.GetScheduleByDoctorAsync(doctorId);
        
        // Assert
        schedule.Should().NotBeNull();
        schedule.DoctorId.Should().Be(doctorId);
        schedule.Schedule.Count.Should().Be(testSchedule.Count);
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
        repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), true))
            .ReturnsAsync(doctor);
        
        // Act
        var schedule = await service.GetScheduleByDoctorAsync(doctorId, dayOfWeek);
        
        // Assert
        schedule.Should().NotBeNull();
        schedule.Schedule.Should().OnlyContain(x => x.DayOfWeek == dayOfWeek);
    }
}