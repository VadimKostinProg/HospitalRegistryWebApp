using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO
{
    /// <summary>
    /// DTO for register request.
    /// </summary>
    public class RegisterDTO : CreateAccountRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
