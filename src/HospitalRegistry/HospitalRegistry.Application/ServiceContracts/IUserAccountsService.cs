using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for user authentification.
    /// </summary>
    public interface IUserAccountsService
    {
        /// <summary>
        /// Method for reading accounts list with applying specifications.
        /// </summary>
        /// <param name="specifications">Specifications to filter, sort, search and paginate accounts.</param>
        /// <returns>Collection IEnumerable of AccountResponse objects.</returns>
        Task<IEnumerable<AccountResponse>> GetAccountsList(Specifications specifications);

        /// <summary>
        /// Method for registering doctors and users accounts.
        /// </summary>
        /// <param name="register">Register infotmation.</param>
        /// <returns>Response of authorization with token.</returns>
        Task<AuthenticationResponse> RegisterAsync(RegisterDTO register);

        /// <summary>
        /// Method for creating new admin or receptionist account.
        /// </summary>
        /// <param name="user">Admins credentianls.</param>
        Task CreateAccount(CreateAccountRequest user);

        /// <summary>
        /// Method for sign in user.
        /// </summary>
        /// <param name="login">User login credentials.</param>
        /// <returns>Response of authorization with token.</returns>
        Task<AuthenticationResponse> LoginAsync(LoginDTO login);

        /// <summary>
        /// Method for deleting user accounts(does not delete records of doctors and patients). 
        /// </summary>
        /// <param name="accountId">Id of account to delete.</param>
        Task DeleteAccount(Guid accountId);

        /// <summary>
        /// Method for logout current user.
        /// </summary>
        Task LogoutAsync();
    }
}
