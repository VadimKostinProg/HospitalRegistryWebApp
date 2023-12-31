using HospitalReqistry.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO;

/// <summary>
/// DTO for doctor information response.
/// </summary>
public class DoctorResponse
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Patronymic { get; set; }

    public string DateOfBirth { get; set; }

    public Specialty Specialty { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }
}

public static partial class ConvertExt
{
    public static DoctorResponse ToDoctorResponse(this Doctor doctor)
    {
        return new DoctorResponse()
        {
            Id = doctor.Id,
            Name = doctor.Name,
            Surname = doctor.Surname,
            Patronymic = doctor.Patronymic,
            DateOfBirth = doctor.DateOfBirth,
            Specialty = (Specialty)Enum.Parse<Specialty>(doctor.Specialty),
            Email = doctor.Email,
            PhoneNumber = doctor.PhoneNumber
        };
    }
} 