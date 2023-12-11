using System.ComponentModel.DataAnnotations;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO for updating name of existant diagnosis request.
/// </summary>
public class DiagnosisUpdateRequest : DiagnosisCreateRequest
{
    [Required]
    public Guid Id { get; set; }

    public override Diagnosis ToDiagnosis()
    {
        var diagnosis = base.ToDiagnosis();
        diagnosis.Id = this.Id;

        return diagnosis;
    }
}