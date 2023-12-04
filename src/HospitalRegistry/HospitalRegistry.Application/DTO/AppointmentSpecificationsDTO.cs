using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO
{
    public class AppointmentSpecificationsDTO : SpecificationsDTO
    {
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? DiagnosisId { get; set; }
        public DateOnly? Date { get; set; }
        public AppointmentType? Type { get; set; }
        public AppointmentStatus? Status { get; set; }
    }
}
