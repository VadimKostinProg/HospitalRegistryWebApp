using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO
{
    public class RegisterDTO : CreateUserDTO
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
