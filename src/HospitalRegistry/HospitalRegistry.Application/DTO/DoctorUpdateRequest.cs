using System.ComponentModel.DataAnnotations;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class DoctorUpdateRequest : DoctorAddRequest
{
    [Required]
    public Guid Id { get; set; }

    public override Doctor ToDoctor()
    {
        var doctor = base.ToDoctor();
        doctor.Id = this.Id;

        return doctor;
    }
}