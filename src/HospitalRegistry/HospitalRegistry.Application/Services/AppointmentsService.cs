using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HospitalRegistry.Application.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly IAsyncRepository _repository;
    private readonly IDoctorsService _doctorsService;
    private readonly ISchedulesService _schedulesService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public AppointmentsService(IAsyncRepository repository, 
        IDoctorsService doctorsService,
        ISchedulesService schedulesService,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _doctorsService = doctorsService;
        _schedulesService = schedulesService;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<IEnumerable<AppointmentResponse>> GetAppointmnetsList(Specifications specifications)
    {
        var appointments = await _repository.GetAsync<Appointment>();

        return appointments.Select(appointment => new AppointmentResponse
        {
            Id = appointment.Id,
            DateAndTime = appointment.DateAndTime,
            Doctor = appointment.Doctor.ToDoctorResponse(),
            Patient = appointment.Patient.ToPatientResponse(),
            AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
            ExtraClinicalData = appointment.ExtraClinicalData,
            Diagnosis = appointment.Diagnosis?.Name,
            Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status),
            Conclusion = appointment.Conclusion
        })
        .ToList();
    }

    public async Task<AppointmentResponse> GetAppointmentById(Guid id)
    {
        var appointment = await _repository.GetByIdAsync<Appointment>(id);

        if (appointment is null)
        {
            throw new KeyNotFoundException("Appointment with such id does not exist.");
        }

        return new AppointmentResponse
        {
            Id = appointment.Id,
            DateAndTime = appointment.DateAndTime,
            Doctor = appointment.Doctor.ToDoctorResponse(),
            Patient = appointment.Patient.ToPatientResponse(),
            AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
            ExtraClinicalData = appointment.ExtraClinicalData,
            Diagnosis = appointment.Diagnosis?.Name,
            Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status),
            Conclusion = appointment.Conclusion
        };
    }

    public async Task<IEnumerable<AppointmentSlotResponse>> SearchFreeSlotsAsync(FreeSlotsSearchSpecifications specifications)
    {
        // Validating the request
        if (specifications is null)
            throw new ArgumentNullException("Specification for searching is null.");

        if (specifications.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Start date must begin from the current day.");

        if (specifications.EndDate < specifications.StartDate)
            throw new ArgumentException("End date must be equal or more than the start date.");

        // List for free slots
        var freeSlots = new List<AppointmentSlotResponse>();

        // List for doctors on which the search will be performed
        var doctors = new List<DoctorResponse>();

        if (specifications.DoctorId is not null)    // If doctors id is passed, search will be perform only for passed doctor
        {
            var doctor = await _doctorsService.GetByIdAsync(specifications.DoctorId.Value);

            if (doctor is null)
                throw new KeyNotFoundException("Doctor with such Id does not exist.");

            doctors.Add(doctor);
        }
        else if (specifications.Specialty is not null)  // If specialty is passed, isntead of doctors id, search will be perform for all doctors of passed specialty
        {
            var doctorsFiltered = await _doctorsService.GetBySpecialtyAsync(specifications.Specialty.Value);
            doctors.AddRange(doctorsFiltered);
        }
        else throw new ArgumentNullException("Doctors specialty and doctors Id cannot be blank both. Pass at least one of the parameters.");

        // Finding free slots for doctors
        foreach (var doctor in doctors)
        {
            var freeSlotsOfDoctot =
                await GetFreeSlotsForDoctor(doctor, specifications.StartDate, specifications.EndDate, specifications.AppointmentType);

            freeSlots.AddRange(freeSlotsOfDoctot);
        }

        return freeSlots
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToList();
    }

    // Method for getting free slots for doctor between passed days for passed appointment type.
    private async Task<IEnumerable<AppointmentSlotResponse>> GetFreeSlotsForDoctor(DoctorResponse doctor, DateOnly startDate, DateOnly endDate, AppointmentType appointmentType)
    {
        // Convert DateOnly to DateTime format
        var startDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day);
        var endDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day);

        // Get all appointments for passed dates
        var appointments =
            (await _repository.GetFilteredAsync<Appointment>(x =>
                x.DateAndTime >= startDateTime && x.DateAndTime <= endDateTime))
            .ToList();

        // Get schedule of the doctor
        var schedule = await _schedulesService.GetScheduleByDoctorAsync(doctor.Id);

        // Filling slots as all range of schedule slots between dates.
        var allSlots = FillScheduleForDates(schedule, startDate, endDate, doctor.Specialty);

        // Filtering only free slots
        var freeSlots = allSlots.Where(x => !AppointmnetAlreadyExistsForTimeSlot(appointments, x));

        return freeSlots;
    }

    // Method for checking if appointment for timeSlot already exists
    private bool AppointmnetAlreadyExistsForTimeSlot(List<Appointment> appointments, AppointmentSlotResponse timeSlot) =>
        appointments.Any(x =>
            x.DateAndTime == timeSlot.Date.ToDateTime(timeSlot.StartTime) &&
            x.DoctorId == timeSlot.DoctorId &&
            x.Status == AppointmentStatus.Scheduled.ToString());

    // Method for filling free slot with doctors schedule and passed dates.
    private List<AppointmentSlotResponse> FillScheduleForDates(ScheduleDTO schedule, DateOnly startDate, DateOnly endDate, Specialty specialty)
    {
        var slots = new List<AppointmentSlotResponse>();

        for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var timeSlotsForDate = schedule.Schedule.Where(x => x.DayOfWeek % 7 == (int)date.DayOfWeek).ToList();
            foreach (var timeSlot in timeSlotsForDate)
            {
                var appointmentSlot = new AppointmentSlotResponse()
                {
                    Date = date,
                    DayOfWeek = timeSlot.DayOfWeek,
                    StartTime = timeSlot.StartTime,
                    EndTime = timeSlot.EndTime,
                    AppointmentType = timeSlot.AppointmentType,
                    Specialty = specialty,
                    DoctorId = schedule.DoctorId
                };

                slots.Add(appointmentSlot);
            }
        }

        return slots;
    }

    public async Task<IEnumerable<AppointmentResponse>> GetAppointmentsHistoryOfPatientAsync(Guid patientId)
    {
        var patient = await _repository.GetByIdAsync<Patient>(patientId);

        if (patient is null)
            throw new KeyNotFoundException("Patient with such Id does not exist.");

        return patient.Appointments
            .Where(appointment => appointment.Status == AppointmentStatus.Completed.ToString())
            .Select(appointment => new AppointmentResponse
            {
                Id = appointment.Id,
                DateAndTime = appointment.DateAndTime,
                Doctor = appointment.Doctor.ToDoctorResponse(),
                Patient = patient.ToPatientResponse(),
                AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
                ExtraClinicalData = appointment.ExtraClinicalData,
                Diagnosis = appointment.Diagnosis?.Name,
                Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status),
                Conclusion = appointment.Conclusion
            })
            .ToList();
    }

    public async Task<IEnumerable<AppointmentResponse>> GetAppointmentsHistoryOfDoctorAsync(Guid doctorId)
    {
        var doctor = await _repository.GetByIdAsync<Doctor>(doctorId);

        if (doctor is null)
            throw new KeyNotFoundException("Doctor with such id does not exits.");

        return doctor.Appointments
            .Where(appointment => appointment.Status == AppointmentStatus.Completed.ToString())
            .Select(appointment => new AppointmentResponse()
            {
                Id = appointment.Id,
                DateAndTime = appointment.DateAndTime,
                Doctor = doctor.ToDoctorResponse(),
                Patient = appointment.Patient.ToPatientResponse(),
                AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
                ExtraClinicalData = appointment.ExtraClinicalData,
                Diagnosis = appointment.Diagnosis?.Name,
                Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status),
                Conclusion = appointment.Conclusion
            })
            .ToList();
    }

    public async Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfPatientAsync(Guid patientId, DateOnly date)
    {
        var patient = await _repository.GetByIdAsync<Patient>(patientId);

        if (patient is null)
            throw new KeyNotFoundException("Patient with such id does not exist.");

        if (patient.IsDeleted)
            throw new ArgumentException("Patient is deleted.");

        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Scheduled appointments can be accessed only for present and future days.");

        var dateAndTime = date.ToDateTime(new TimeOnly(0, 0));

        var appointments =
            await _repository.GetFilteredAsync<Appointment>(x =>
                x.PatientId == patientId &&
                x.Status == AppointmentStatus.Scheduled.ToString() &&
                x.DateAndTime >= dateAndTime);

        return appointments.Select(appointment => new AppointmentResponse()
        {
            Id = appointment.Id,
            DateAndTime = appointment.DateAndTime,
            Doctor = appointment.Doctor.ToDoctorResponse(),
            Patient = patient.ToPatientResponse(),
            AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
            ExtraClinicalData = appointment.ExtraClinicalData,
            Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status)
        })
            .ToList();
    }

    public async Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfDoctorAsync(Guid doctorId, DateOnly date)
    {
        var doctor = await _repository.GetByIdAsync<Doctor>(doctorId);

        if (doctor is null)
            throw new KeyNotFoundException("Doctor with such id does not exist.");

        if (doctor.IsDeleted)
            throw new ArgumentException("Doctor is deleted.");

        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Scheduled appointments can be accessed only for present and future days.");

        var dateAndTime = date.ToDateTime(new TimeOnly(0, 0));

        var appointments =
            await _repository.GetFilteredAsync<Appointment>(x =>
                x.DoctorId == doctorId &&
                x.Status == AppointmentStatus.Scheduled.ToString() &&
                x.DateAndTime >= dateAndTime);

        return appointments.Select(appointment => new AppointmentResponse()
        {
            Id = appointment.Id,
            DateAndTime = appointment.DateAndTime,
            Doctor = doctor.ToDoctorResponse(),
            Patient = appointment.Patient.ToPatientResponse(),
            AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
            ExtraClinicalData = appointment.ExtraClinicalData,
            Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status)
        })
            .ToList();
    }

    public async Task SetAppointmentAsync(AppointmentSetRequest request)
    {
        // Validating request
        if (request == null)
            throw new ArgumentNullException("Appointment to set is null.");

        if (request.DateAndTime <= DateTime.UtcNow)
            throw new ArgumentException("Date and time must be more than now.");

        var doctor = await _repository.GetByIdAsync<Doctor>(request.DoctorId);

        if (doctor is null)
            throw new KeyNotFoundException("Doctor with such id does not exist.");

        if (doctor.IsDeleted)
            throw new ArgumentException("Doctor is deleted.");

        var patient = await _repository.GetByIdAsync<Patient>(request.PatientId);

        if (patient is null)
            throw new KeyNotFoundException("Patient with such id does not exist.");

        if (patient.IsDeleted)
            throw new ArgumentException("Patient is deleted.");

        if (await _repository.ContainsAsync<Appointment>(x => x.DateAndTime == request.DateAndTime &&
                                                              x.DoctorId == request.DoctorId &&
                                                              x.Status == AppointmentStatus.Scheduled.ToString()))
            throw new ArgumentException("Another appointmnet has been already set on this time.");

        // Adding new appointment
        var appointment = new Appointment()
        {
            DateAndTime = request.DateAndTime,
            DoctorId = request.DoctorId,
            PatientId = request.PatientId,
            ExtraClinicalData = request.ExtraClinicalData
        };
        await _repository.AddAsync(appointment);
    }

    public async Task CompleteAppointmentAsync(AppointmentCompleteRequest request)
    {
        // Validating request
        if (request is null)
            throw new ArgumentNullException("Appointment to complete is null.");

        var appointment = await _repository.GetByIdAsync<Appointment>(request.Id);

        if (appointment is null)
            throw new KeyNotFoundException("Appointment with such id does not exist.");

        if (appointment.Status == AppointmentStatus.Completed.ToString())
            throw new ArgumentException("Appointment is already completed.");

        if (appointment.Status == AppointmentStatus.Canceled.ToString())
            throw new ArgumentException("Cannot compltete canceled appointment.");

        var diagnosis = await _repository.GetByIdAsync<Diagnosis>(request.DiagnosisId);

        if (diagnosis is null)
            throw new KeyNotFoundException("Diagnosis with such id does not exist.");

        if (diagnosis.IsDeleted)
            throw new ArgumentException("Cannot set deleted diagnoses.");

        if (string.IsNullOrEmpty(request.Conclusion))
            throw new ArgumentNullException("Conclusion cannot be blank.");

        // Validate user
        var currentUserClaims = _httpContextAccessor.HttpContext?.User;

        var user = await _userManager.GetUserAsync(currentUserClaims);

        if (user!.DoctorId != appointment.DoctorId)
        {
            throw new ArgumentException("Cannot complete appointment of other doctor.");
        }

        // Completing appointment
        appointment.DiagnosisId = request.DiagnosisId;
        appointment.Conclusion = request.Conclusion;
        appointment.Status = AppointmentStatus.Completed.ToString();

        await _repository.UpdateAsync(appointment);
    }

    public async Task CancelAppointmentAsync(Guid appointmentId)
    {
        // Validating request
        var appointment = await _repository.GetByIdAsync<Appointment>(appointmentId);

        if (appointment is null)
            throw new KeyNotFoundException("Appointment with such id does not exist.");

        if (appointment.Status == AppointmentStatus.Canceled.ToString())
            throw new ArgumentException("Appointment has already been canceled.");

        if (appointment.Status == AppointmentStatus.Completed.ToString())
            throw new ArgumentException("Cannot cancel already completed appointment");

        // Canceling appointment
        appointment.Status = AppointmentStatus.Canceled.ToString();
        await _repository.UpdateAsync(appointment);
    }

    public async Task RecoverAsync(Guid id)
    {
        // Validating request
        var appointment = await _repository.GetByIdAsync<Appointment>(id);

        if (appointment is null)
            throw new KeyNotFoundException("Appointment with such id does not exist.");

        if (appointment.Status != AppointmentStatus.Canceled.ToString())
            throw new ArgumentException("Appointment is not canceled to recover.");

        int maxDelay = AppointmentsConfiguration.MaxAppointmentRecoveringLateness;
        if (appointment.DateAndTime.AddMinutes(maxDelay) < DateTime.UtcNow)
            throw new ArgumentException($"Cannot recover appointment that is more than {maxDelay} minutes late from schedule.");

        if (await _repository.ContainsAsync<Appointment>(x => x.DateAndTime == appointment.DateAndTime &&
                                                              x.DoctorId == appointment.DoctorId &&
                                                              x.Status == AppointmentStatus.Scheduled.ToString()))
            throw new ArgumentException("Another appointmnet has been already set on this time.");

        // Recovering appointment
        appointment.Status = AppointmentStatus.Scheduled.ToString();
        await _repository.UpdateAsync(appointment);
    }

    public async Task ClearAllCanceledAppointmentsAsync()
    {
        var canceledAppointments =
            await _repository.GetFilteredAsync<Appointment>(x => x.Status == AppointmentStatus.Canceled.ToString());

        await _repository.DeleteRangeAsync(canceledAppointments);
    }
}