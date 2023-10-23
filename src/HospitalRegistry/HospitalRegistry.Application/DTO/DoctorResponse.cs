using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class DoctorResponse
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Patronymic { get; set; }

    public string Specialty { get; set; }

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
            Specialty = doctor.Specialty,
            Email = doctor.Email,
            PhoneNumber = "+" + doctor.PhoneNumber.ToString()
        };
    }
} 