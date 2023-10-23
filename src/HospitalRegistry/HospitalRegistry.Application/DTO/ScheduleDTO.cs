using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO;

public class ScheduleDTO
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required] 
    [MinLength(1)] 
    public List<TimeSlotDTO> Schedule { get; set; } = null!;
}