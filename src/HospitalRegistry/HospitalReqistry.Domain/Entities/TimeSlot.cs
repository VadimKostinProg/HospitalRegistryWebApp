using System.ComponentModel.DataAnnotations;

namespace HospitalReqistry.Domain.Entities
{
    public class TimeSlot : EntityBase
    {
        [Required]
        [Range(1, 7)]
        public int DayOfWeek { get; set; }

        [Required]
        [MaxLength(5)]
        public string StartTime { get; set; } = null!;

        [Required]
        [MaxLength(5)]
        public string EndTime { get; set; } = null!;
    }
}
