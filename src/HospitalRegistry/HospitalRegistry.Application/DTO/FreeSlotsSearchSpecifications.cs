using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO
{
    /// <summary>
    /// DTO for specifications to search free slots of appointment request.
    /// </summary>
    public class FreeSlotsSearchSpecifications
    {
        /// <summary>
        /// Start date to search free slots.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// End date to search free slots.
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// AppointmentType to search free slots.
        /// </summary>
        public AppointmentType AppointmentType { get; set; }

        /// <summary>
        /// Specialty of the doctor to search free slots.
        /// </summary>
        public Specialty? Specialty { get; set; }

        /// <summary>
        /// Doctors id to search free slots.
        /// (Optional)
        /// </summary>
        public Guid? DoctorId { get; set; }
    }
}
