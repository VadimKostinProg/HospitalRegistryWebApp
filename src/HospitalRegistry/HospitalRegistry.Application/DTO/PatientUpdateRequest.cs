using System.ComponentModel.DataAnnotations;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO for updating patient information request.
/// </summary>
public class PatientUpdateRequest : PatientAddRequest
{
    [Required]
    public Guid Id { get; set; }

    public override Patient ToPatient()
    {
        var patient = base.ToPatient();
        patient.Id = this.Id;

        return patient;
    }
}