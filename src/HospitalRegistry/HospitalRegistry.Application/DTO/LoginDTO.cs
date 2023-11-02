using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
