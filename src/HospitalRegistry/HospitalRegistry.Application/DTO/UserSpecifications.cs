using System.ComponentModel.DataAnnotations;

namespace HospitalRegistry.Application.DTO
{
    public class UserSpecifications
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Surname { get; set; } = null!;

        [Required]
        public string Patronymic { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;
    }
}
