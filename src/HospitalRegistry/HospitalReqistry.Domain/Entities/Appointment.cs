using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalReqistry.Domain.Entities
{
    public class Appointment : EntityBase
    {
        [Required]
        public DateTime DateAndTime { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        public Guid? DiagnosisId { get; set; }

        [MaxLength(70)]
        public string? ExtraClinicalData { get; set; }

        [Required]
        [MaxLength(15)]
        public string Status { get; set; } = null!;

        [MaxLength(500)]
        public string? Conclusion { get; set; } = null!;

        [ForeignKey(nameof(DoctorId))]
        public virtual Doctor Doctor { get; set; }

        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; }

        [ForeignKey(nameof(DiagnosisId))]
        public virtual Diagnosis? Diagnosis { get; set; }
    }
}
