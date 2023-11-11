using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO for diagnosis information response.
/// </summary>
public class DiagnosisResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public static partial class ConvertExt
{
    public static DiagnosisResponse ToDiagnosisResponse(this Diagnosis diagnosis)
    {
        return new DiagnosisResponse()
        {
            Id = diagnosis.Id,
            Name = diagnosis.Name
        };
    }
} 