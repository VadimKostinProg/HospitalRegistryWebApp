using System.ComponentModel.DataAnnotations;

namespace HospitalReqistry.Domain.Entities
{
    public class Doctor : EntityBase
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
        public string Specialty { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Email { get; set; } = null!;

        [Required]
        public int PhoneNumber { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
