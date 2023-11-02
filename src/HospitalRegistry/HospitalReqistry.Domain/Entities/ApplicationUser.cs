using Microsoft.AspNetCore.Identity;

namespace HospitalReqistry.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }
    }
}
