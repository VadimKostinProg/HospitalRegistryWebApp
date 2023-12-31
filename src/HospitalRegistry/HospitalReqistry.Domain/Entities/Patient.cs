﻿using System.ComponentModel.DataAnnotations;

namespace HospitalReqistry.Domain.Entities
{
    public class Patient : EntityBase
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Surname { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Patronymic { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string DateOfBirth { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
