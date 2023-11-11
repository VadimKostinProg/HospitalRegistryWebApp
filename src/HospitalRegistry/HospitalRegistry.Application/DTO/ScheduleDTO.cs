using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO for schedule of the specific doctor information.
/// </summary>
public class ScheduleDTO
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required] 
    [MinLength(1)] 
    public List<TimeSlotDTO> Schedule { get; set; } = null!;
}