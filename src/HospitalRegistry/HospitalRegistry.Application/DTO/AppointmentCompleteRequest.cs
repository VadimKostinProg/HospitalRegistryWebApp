using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO for complete of specific appointment request.
/// </summary>
public class AppointmentCompleteRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid DiagnosisId { get; set; }

    [Required] 
    [MaxLength(500)]
    public string Conclusion { get; set; } = null!;
}