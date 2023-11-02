using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for user authentification.
    /// </summary>
    public interface IUserAuthService
    {
        /// <summary>
        /// Method for registering doctors and users accounts.
        /// </summary>
        /// <param name="register">Register infotmation.</param>
        /// <returns>Response of authorization with token.</returns>
        Task<AuthorizationResponse> RegisterAsync(RegisterDTO register);

        /// <summary>
        /// Method for sign in user.
        /// </summary>
        /// <param name="login">User login credentials.</param>
        /// <returns>Response of authorization with token.</returns>
        Task<AuthorizationResponse> LoginAsync(LoginDTO login);

        /// <summary>
        /// Method for logout user.
        /// </summary>
        Task LogoutAsync();
    }
}
