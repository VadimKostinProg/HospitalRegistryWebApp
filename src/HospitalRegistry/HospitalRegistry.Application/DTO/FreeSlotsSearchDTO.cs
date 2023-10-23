using System.ComponentModel.DataAnnotations;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO;

public class FreeSlotsSearchDTO : TimeSlotDTO
{
    [Required]
    public Specialty Specialty { get; set; }
    
    public Guid? DoctorsId { get; set; }
}