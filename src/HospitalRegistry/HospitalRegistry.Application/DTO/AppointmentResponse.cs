using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO for appointment general information response.
/// </summary>
public class AppointmentResponse
{
    public Guid Id { get; set; }
    
    public DateTime DateAndTime { get; set; }
    
    public DoctorResponse Doctor { get; set; }
    
    public PatientResponse Patient { get; set; }
    
    public AppointmentType AppointmentType { get; set; }
    
    public string? Diagnosis { get; set; }
    
    public string? ExtraClinicalData { get; set; }
    
    public AppointmentStatus Status { get; set; }
    
    public string? Conclusion { get; set; }
}