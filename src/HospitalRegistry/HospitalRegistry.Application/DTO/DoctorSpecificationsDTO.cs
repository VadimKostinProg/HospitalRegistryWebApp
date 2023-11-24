using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO
{
    public class DoctorSpecificationsDTO : PatientSpecificationsDTO
    {
        public Specialty? Specialty { get; set; }
    }
}
