using System.ComponentModel.DataAnnotations;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO;

public class AppointmentSlotResponse : TimeSlotDTO
{
    public DateOnly Date { get; set; }

    public Specialty Specialty { get; set; }
    
    public Guid DoctorId { get; set; }
}