using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO
{
    public class PatientSpecificationsDTO : SpecificationsDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }
}
