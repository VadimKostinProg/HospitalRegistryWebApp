using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO to set new appointment request.
/// </summary>
public class AppointmentSetRequest
{
    [Required]
    public DateTime DateAndTime { get; set; }
    
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid PatientId { get; set; }
    
    public string? ExtraClinicalData { get; set; }
}