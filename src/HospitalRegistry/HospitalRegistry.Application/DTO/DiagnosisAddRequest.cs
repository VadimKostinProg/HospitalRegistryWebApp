using System.ComponentModel.DataAnnotations;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class DiagnosisAddRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public virtual Diagnosis ToDiagnosis() => new Diagnosis() { Name = this.Name };
}