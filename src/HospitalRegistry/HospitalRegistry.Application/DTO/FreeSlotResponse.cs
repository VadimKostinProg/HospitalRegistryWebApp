using System.ComponentModel.DataAnnotations;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO;

public class FreeSlotResponse : TimeSlotDTO
{
    public DateOnly Date { get; set; }

    public Specialty Specialty { get; set; }
    
    public Guid DoctorsId { get; set; }
}