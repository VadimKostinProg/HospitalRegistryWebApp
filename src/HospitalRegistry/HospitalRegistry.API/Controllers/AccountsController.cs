using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRegistry.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserAccountsService _userAccountsService;

        public AccountsController(IUserAccountsService userAccountsService)
        {
            _userAccountsService = userAccountsService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<ListModel<AccountResponse>>> GetAccountsList([FromQuery] AccountSpecificationsDTO specifications)
        {
            return Ok(await _userAccountsService.GetAccountsListAsync(specifications));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> CreateUser([FromBody] CreateAccountRequest createUserRequest)
        {
            await _userAccountsService.CreateAccountAsync(createUserRequest);

            return Ok("User has been successfully created.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginDTO login)
        {
            return Ok(await _userAccountsService.LoginAsync(login));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> Register([FromBody] RegisterDTO register)
        {
            return Ok(await _userAccountsService.RegisterAsync(register));
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await _userAccountsService.LogoutAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> DeleteAccount([FromRoute] Guid id)
        {
            await _userAccountsService.DeleteAccountAsync(id);

            return Ok("Account has been deleted successfully.");
        }
    }
}