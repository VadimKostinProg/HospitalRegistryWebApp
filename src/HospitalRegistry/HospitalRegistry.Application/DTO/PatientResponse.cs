using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class PatientResponse
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Patronymic { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }
}

public static partial class ConvertExt
{
    public static PatientResponse ToPatientResponse(this Patient patient)
    {
        return new PatientResponse()
        {
            Id = patient.Id,
            Name = patient.Name,
            Surname = patient.Surname,
            Patronymic = patient.Patronymic,
            Email = patient.Email,
            PhoneNumber = "+" + patient.PhoneNumber.ToString()
        };
    }
} 