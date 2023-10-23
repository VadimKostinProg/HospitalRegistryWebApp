using System.ComponentModel.DataAnnotations;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO;

public class TimeSlotDTO
{
    [Required]
    public TimeOnly StartTime { get; set; }
    
    [Required]
    public TimeOnly EndTime { get; set; }

    [Required] 
    public AppointmentType AppointmentType { get; set; }
}