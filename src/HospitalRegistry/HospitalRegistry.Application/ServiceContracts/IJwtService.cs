using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for creating JSON web tokens.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Method for creating JSON web token for particular user.
        /// </summary>
        /// <param name="user">User to create JWT.</param>
        /// <returns>JSON web token for passed user.</returns>
        Task<string> CreateJwtToken(ApplicationUser user);
    }
}
