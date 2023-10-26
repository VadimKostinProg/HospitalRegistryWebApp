using System.ComponentModel.DataAnnotations;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class TimeSlotDTO
{
    [Required]
    public TimeOnly StartTime { get; set; }
    
    [Required]
    public TimeOnly EndTime { get; set; }
    
    [Required]
    [Range(1, 7)]
    public int DayOfWeek { get; set; }

    [Required] 
    public AppointmentType AppointmentType { get; set; }
}

public static partial class ConvertExt
{
    public static TimeSlotDTO ToTimeSlotDTO(this Schedule schedule)
    {
        string[] startTimeArgs = schedule.TimeSlot.StartTime.Split(':');
        string[] endTimeArgs = schedule.TimeSlot.EndTime.Split(':');

        return new TimeSlotDTO()
        {
            StartTime = new TimeOnly(int.Parse(startTimeArgs[0]), int.Parse(startTimeArgs[1])),
            EndTime = new TimeOnly(int.Parse(endTimeArgs[0]), int.Parse(endTimeArgs[1])),
            DayOfWeek = schedule.TimeSlot.DayOfWeek,
            AppointmentType = (AppointmentType)Enum.Parse(typeof(AppointmentType), schedule.AppointmentType)
        };
    }
}