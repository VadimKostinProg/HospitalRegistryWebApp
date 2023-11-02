using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Surname { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
