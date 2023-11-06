using Microsoft.AspNetCore.Identity;

namespace HospitalReqistry.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }
    }
}
