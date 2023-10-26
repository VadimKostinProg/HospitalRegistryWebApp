using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Moq;

namespace HospitalRegistry.Tests.SchedulesServiceTests;

public abstract class SchedulesServiceTestsBase
{
    protected readonly ISchedulesService service;
    protected readonly Mock<IAsyncRepository> repositoryMock;
    protected readonly Fixture fixture;

    public SchedulesServiceTestsBase()
    {
        fixture = new Fixture();

        repositoryMock = new Mock<IAsyncRepository>();

        service = new SchedulesService(repositoryMock.Object);
    }

    public IEnumerable<Schedule> GetTestSchedules(Guid doctorId, int startHour = 10, int endHour = 15)
    {
        for (int i = startHour; i <= endHour; i++)
        {
            var timeSlot = new TimeSlot()
            {
                Id = Guid.NewGuid(),
                StartTime = $"{i}:00",
                EndTime = $"{i + 1}:00",
                DayOfWeek = 1
            };

            yield return new Schedule()
            {
                Id = Guid.NewGuid(),
                AppointmentType = AppointmentType.Consultation.ToString(),
                DoctorId = doctorId,
                TimeSlotId = timeSlot.Id,
                TimeSlot = timeSlot
            };
        }
    }

    public IEnumerable<TimeSlotDTO> GetTestCrossedTimeSlotDTO(Guid doctorId)
    {
        yield return new TimeSlotDTO()
        {
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            DayOfWeek = 1,
            AppointmentType = AppointmentType.Consultation
        };
        yield return new TimeSlotDTO()
        {
            StartTime = new TimeOnly(10, 30),
            EndTime = new TimeOnly(11, 30),
            DayOfWeek = 1,
            AppointmentType = AppointmentType.Consultation
        };
    }
}