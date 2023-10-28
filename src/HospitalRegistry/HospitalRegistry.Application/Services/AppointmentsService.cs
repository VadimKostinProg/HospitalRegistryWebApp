using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.Helpers;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;

namespace HospitalRegistry.Application.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly IAsyncRepository _repository;
    private readonly IDoctorsService _doctorsService;
    private readonly ISchedulesService _schedulesService;

    public AppointmentsService(IAsyncRepository repository, IDoctorsService doctorsService, ISchedulesService schedulesService)
    {
        _repository = repository;
        _doctorsService = doctorsService;
        _schedulesService = schedulesService;
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
            x.DateAndTime == timeSlot.Date.ToDateTime(timeSlot.StartTime) && x.DoctorId == timeSlot.DoctorId);
    
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
                Patient = appointment.Patient.ToPatientResponse(),
                AppointmentType = GetAppointment(appointment),
                ExtraClinicalData = appointment.ExtraClinicalData,
                Diagnosis = appointment.Diagnosis?.Name,
                Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status),
                Conclusion = appointment.Conclusion
            })
            .ToList();
    }

    public Task<IEnumerable<AppointmentResponse>> GetAppointmentsHistoryOfDoctorAsync(Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfPatientAsync(Guid patientId, DateOnly date)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfDoctorAsync(Guid doctorId, DateOnly date)
    {
        throw new NotImplementedException();
    }

    private AppointmentType GetAppointment(Appointment appointment)
    {
        var appointmentType = appointment.Doctor.Schedules
            .FirstOrDefault(x => x.TimeSlot.DayOfWeek % 7 == (int)appointment.DateAndTime.DayOfWeek &&
            x.TimeSlot.StartTime == appointment.DateAndTime.ToString("hh:mm"))
            ?.AppointmentType;

        return appointmentType is null ? AppointmentType.HealthVisit : 
            (AppointmentType)Enum.Parse<AppointmentType>(appointmentType);
    }

    public Task SetAppointmentAsync(AppointmentSetRequest request)
    {
        throw new NotImplementedException();
    }

    public Task CompleteAppointmentAsync(AppointmentCompleteRequest request)
    {
        throw new NotImplementedException();
    }

    public Task CancelAppointmentAsync(Guid appointmentId)
    {
        throw new NotImplementedException();
    }

    public Task RecoverAppointmentAsync(Guid appointmentId)
    {
        throw new NotImplementedException();
    }

    public Task ClearAllCanceledAppointmentsAsync()
    {
        throw new NotImplementedException();
    }
}