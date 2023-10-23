using System.ComponentModel.DataAnnotations;

namespace HospitalReqistry.Domain.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public Guid Id { get; set; }

        public EntityBase()
        {
            Id = Guid.NewGuid();
        }
    }
}
