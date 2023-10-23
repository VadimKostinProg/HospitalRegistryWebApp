using System.ComponentModel.DataAnnotations;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO;

public class DoctorAddRequest
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
    public Specialty Specialty { get; set; }

    [Required]
    [MaxLength(30)]
    public string Email { get; set; } = null!;

    [Required] 
    [Phone] 
    public string PhoneNumber { get; set; } = null!;

    public virtual Doctor ToDoctor()
    {
        return new Doctor()
        {
            Name = this.Name,
            Surname = this.Surname,
            Patronymic = this.Patronymic,
            Specialty = this.Specialty.ToString(),
            Email = this.Email,
            PhoneNumber = int.Parse(this.PhoneNumber.Replace("+", "").Replace("-", ""))
        };
    }
}