using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Moq;

namespace HospitalRegistry.Tests;

public abstract class HospitalRegistryTestsBase
{
    protected Mock<IAsyncRepository> repositoryMock = new();
    protected Fixture fixture = new();

    public IEnumerable<Doctor> GetTestDoctors(int count = 10)
    {
        for (int i = 0; i < count; i++)
            yield return GetTestDoctor();
    }

    public Doctor GetTestDoctor()
    {
        return new Doctor
        {
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>(),
            Surname = fixture.Create<string>(),
            Patronymic = fixture.Create<string>(),
            DateOfBirth = "2000.01.01",
            Specialty = Specialty.Allergist.ToString(),
            Email = fixture.Create<string>(),
            PhoneNumber = fixture.Create<string>(),
        };
    }
    
    public IEnumerable<Patient> GetTestPatients(int count = 10)
    {
        for (int i = 0; i < count; i++)
        {
            yield return GetTestPatient();
        }
    }

    public Patient GetTestPatient()
    {
        return new Patient()
        {
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>(),
            Surname = fixture.Create<string>(),
            Patronymic = fixture.Create<string>(),
            DateOfBirth = new DateOnly(2000, 1, 1).ToString(),
            Email = fixture.Create<string>(),
            PhoneNumber = fixture.Create<string>()
        };
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

    public IEnumerable<TimeSlotDTO> GetTestTimeSlotsDTO(int startHour = 10, int endHour = 15)
    {
        for (int dayOfWeek = 1; dayOfWeek <= 5; dayOfWeek++)
        {
            for (int i = startHour; i < endHour; i++)
            {
                yield return new TimeSlotDTO()
                {
                    StartTime = new TimeOnly(i, 0),
                    EndTime = new TimeOnly(i + 1, 0),
                    AppointmentType = AppointmentType.Consultation,
                    DayOfWeek = dayOfWeek
                };
            }
        }
    }

    public IEnumerable<TimeSlotDTO> GetTestCrossedTimeSlotsDTO()
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

    public ScheduleDTO GetTestScheduleDTO(Guid doctorId)
    {
        return new ScheduleDTO()
        {
            DoctorId = doctorId,
            Schedule = GetTestTimeSlotsDTO().ToList()
        };
    }
    
    public IEnumerable<Diagnosis> GetDiagnoses(int count = 10)
    {
        for (int i = 0; i < count; i++)
            yield return GetDiagnosis();
    }

    public Diagnosis GetDiagnosis()
    {
        return new Diagnosis()
        {
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>()
        };
    }
}