using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HospitalRegistry.Tests;

public abstract class HospitalRegistryTestsBase
{
    protected readonly Mock<IAsyncRepository> repositoryMock;
    protected readonly Fixture fixture;

    public HospitalRegistryTestsBase()
    {
        repositoryMock = new Mock<IAsyncRepository>();
        fixture = new Fixture();
    }

    public IEnumerable<Doctor> GetTestDoctors(int count = 10, bool isDeleted = false)
    {
        for (int i = 0; i < count; i++)
            yield return GetTestDoctor(isDeleted);
    }

    public Doctor GetTestDoctor(bool isDeleted = false)
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
            IsDeleted = isDeleted
        };
    }

    public IEnumerable<Patient> GetTestPatients(int count = 10, bool isDeleted = false)
    {
        for (int i = 0; i < count; i++)
        {
            yield return GetTestPatient(isDeleted);
        }
    }

    public Patient GetTestPatient(bool isDeleted = false)
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

    public IEnumerable<Schedule> GetTestSchedules(Guid doctorId, int dayOfWeek = 1, int startHour = 10, int endHour = 15)
    {
        for (int i = startHour; i <= endHour; i++)
        {
            var timeSlot = new TimeSlot()
            {
                Id = Guid.NewGuid(),
                StartTime = $"{i}:00",
                EndTime = $"{i + 1}:00",
                DayOfWeek = dayOfWeek
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

    public IEnumerable<Diagnosis> GetTestDiagnoses(int count = 10, bool isDeleted = false)
    {
        for (int i = 0; i < count; i++)
            yield return GetTestDiagnosis(isDeleted);
    }

    public Diagnosis GetTestDiagnosis(bool isDeleted = false)
    {
        return new Diagnosis()
        {
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>(),
            IsDeleted = isDeleted
        };
    }

    public IEnumerable<Appointment> GetTestScheduledAppointments(Guid doctorId, Guid patietId, DateOnly startDate, TimeOnly startTime)
    {
        for (DateOnly date = startDate; date <= startDate.AddDays(2); date = date.AddDays(1))
        {
            yield return new Appointment()
            {
                Id = Guid.NewGuid(),
                DateAndTime = date.ToDateTime(startTime),
                DoctorId = doctorId,
                PatientId = patietId,
                AppointmentType = AppointmentType.Consultation.ToString(),
                ExtraClinicalData = fixture.Create<string>(),
                Status = AppointmentStatus.Scheduled.ToString()
            };
        }
    }
    
    public IEnumerable<Appointment> GetTestScheduledAppointments(Doctor doctor, Patient patient, DateOnly date, TimeOnly startTime)
    {
        for (TimeOnly time = startTime; time <= startTime.AddHours(4); time = time.AddHours(1))
        {
            yield return new Appointment()
            {
                Id = Guid.NewGuid(),
                DateAndTime = date.ToDateTime(time),
                DoctorId = doctor.Id,
                PatientId = patient.Id,
                AppointmentType = AppointmentType.Consultation.ToString(),
                ExtraClinicalData = fixture.Create<string>(),
                Status = AppointmentStatus.Scheduled.ToString(),
                Doctor = doctor,
                Patient = patient
            };
        }
    }

    public IEnumerable<Appointment> GetTestCompletedAppointments(Doctor doctor, Patient patient, Diagnosis diagnosis, TimeOnly startTime, TimeOnly endTime)
    {
        var date = new DateOnly(2023, 10, 23);

        for (TimeOnly time = startTime; time <= endTime; time = time.AddHours(1))
        {
            yield return new Appointment()
            {
                Id = Guid.NewGuid(),
                DateAndTime = date.ToDateTime(time),
                DoctorId = doctor.Id,
                PatientId = patient.Id,
                AppointmentType = AppointmentType.Consultation.ToString(),
                ExtraClinicalData = fixture.Create<string>(),
                DiagnosisId = diagnosis.Id,
                Status = AppointmentStatus.Completed.ToString(),
                Conclusion = fixture.Create<string>(),
                Doctor = doctor,
                Patient = patient,
                Diagnosis = diagnosis
            };
        }
    }
}