using System.ComponentModel.DataAnnotations;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class DiagnosisUpdateRequest : DiagnosisAddRequest
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