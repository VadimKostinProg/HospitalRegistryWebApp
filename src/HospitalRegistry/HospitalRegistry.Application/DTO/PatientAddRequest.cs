using System.ComponentModel.DataAnnotations;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class PatientAddRequest
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Surname { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Patronymic { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string Email { get; set; } = null!;

    [Required] 
    public string PhoneNumber { get; set; } = null!;

    public virtual Patient ToPatient()
    {
        return new Patient()
        {
            Name = this.Name,
            Surname = this.Surname,
            Patronymic = this.Patronymic,
            Email = this.Email,
            PhoneNumber = int.Parse(this.PhoneNumber.Replace("+", "").Replace("-", ""))
        };
    }
}