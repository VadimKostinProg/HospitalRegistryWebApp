using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalReqistry.Domain.Entities
{
    public class Schedule : EntityBase
    {
        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid TimeSlotId { get; set; }

        [Required]
        [MaxLength(15)]
        public string AppointmentType { get; set; } = null!;

        [ForeignKey(nameof(TimeSlotId))]
        public virtual TimeSlot TimeSlot { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public virtual Doctor Doctor { get; set; }
    }
}
