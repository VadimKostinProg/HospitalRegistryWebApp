using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Helpers;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;

namespace HospitalRegistry.Application.Services;

public class SchedulesService : ISchedulesService
{
    private readonly IAsyncRepository _repository;

    public SchedulesService(IAsyncRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScheduleDTO> GetScheduleByDoctorAsync(Guid doctorId, int? dayOfWeek = null)
    {
        var doctor = await _repository.GetByIdAsync<Doctor>(doctorId);

        if (doctor is null)
            throw new KeyNotFoundException("Doctor with such id does not exists.");

        var schedule = doctor.Schedules.Select(x => x.ToTimeSlotDTO());

        if (dayOfWeek is not null)
        {
            if (dayOfWeek < 1 || dayOfWeek > 7)
                throw new ArgumentException("Day of week must be between 1 and 7.");

            schedule = schedule.Where(x => x.DayOfWeek == dayOfWeek.Value);
        }

        return new ScheduleDTO()
        {
            DoctorId = doctorId,
            Schedule = schedule.ToList()
        };
    }

    public async Task SetAsync(ScheduleDTO request)
    {
        // Validating request
        if (request is null)
            throw new ArgumentNullException("Schedule to set is null.");

        var doctor = await _repository.GetByIdAsync<Doctor>(request.DoctorId);

        if (doctor is null)
            throw new KeyNotFoundException("Doctor with such Id does not exist.");

        if (!request.Schedule.Any())
            throw new ArgumentException("Schedule to insert must contain at least one time slot.");

        if (!ValidateScheduleSetRequestForTimeSlotsCrossing(request))
            throw new ArgumentException("Time slots of schedule must not cross with each other.");
        
        // Deleting old shedule for doctor
        var daysOfWeek = request.Schedule
            .Select(x => x.DayOfWeek)
            .Distinct()
            .ToArray();
        
        await DeleteOldScheduleForDoctorAsync(doctor, daysOfWeek);
        
        // Adding new schedule
        foreach (var timeSlotDTO in request.Schedule)
        {
            var timeSlotId = await this.GetTimeSlotId(timeSlotDTO);

            var scheduleToInsert = new Schedule()
            {
                DoctorId = request.DoctorId,
                TimeSlotId = timeSlotId,
                AppointmentType = timeSlotDTO.AppointmentType.ToString()
            };

            await _repository.AddAsync(scheduleToInsert);
        }
    }

    // Method that deletes old scheduled of the doctor for the passed days of week.
    private async Task DeleteOldScheduleForDoctorAsync(Doctor doctor, params int[] daysOfWeek)
    {
        foreach (var dayOfWeek in daysOfWeek)
        {
            var schedulesToDelete = doctor.Schedules
                .Where(x => x.TimeSlot.DayOfWeek == dayOfWeek)
                .ToList();

            await _repository.DeleteRangeAsync(schedulesToDelete);
        }
    }

    // Method that validates time slots for crossing.
    private bool ValidateScheduleSetRequestForTimeSlotsCrossing(ScheduleDTO request)
    {
        for (int i = 0; i < request.Schedule.Count - 1; i++)
            for(int j = i + 1; j < request.Schedule.Count; j++)
                if (TimeHelper.TimeSlotsCross(request.Schedule[i], request.Schedule[j]))
                    return false;
        
        return true;
    }
    
    // Method searchs the time slots in DB that fits the parameters of the passed TimeSlotDTO.
    // If there is not any time slot, method creates new one.
    private async Task<Guid> GetTimeSlotId(TimeSlotDTO timeSlotDTO)
    {
        // Search for time slot
        var timeSlot = await _repository
            .FirstOrDefaultAsync<TimeSlot>(x =>
                x.DayOfWeek == timeSlotDTO.DayOfWeek &&
                x.StartTime == timeSlotDTO.StartTime.ToString() &&
                x.EndTime == timeSlotDTO.EndTime.ToString());

        // If time slot does not exist, create new one
        if (timeSlot is null)
        {
            timeSlot = new TimeSlot()
            {
                StartTime = timeSlotDTO.StartTime.ToString(),
                EndTime = timeSlotDTO.EndTime.ToString(),
                DayOfWeek = timeSlotDTO.DayOfWeek
            };

            await _repository.AddAsync(timeSlot);
        }

        return timeSlot.Id;
    }
}