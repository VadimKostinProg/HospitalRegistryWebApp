using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.DTO
{
    /// <summary>
    /// DTO for account list.
    /// </summary>
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        /// <summary>
        /// Id of doctor or patient record, null - for admin and recipients.
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
