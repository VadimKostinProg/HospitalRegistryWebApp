using System.ComponentModel.DataAnnotations;

namespace HospitalReqistry.Domain.Entities
{
    public class Diagnosis : EntityBase
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
