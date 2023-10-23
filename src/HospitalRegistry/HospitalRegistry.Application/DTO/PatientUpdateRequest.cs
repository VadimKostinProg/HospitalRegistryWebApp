using System.ComponentModel.DataAnnotations;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

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