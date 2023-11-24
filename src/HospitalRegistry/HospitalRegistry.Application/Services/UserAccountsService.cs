using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Application.Services
{
    public class UserAccountsService : IUserAccountsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IAsyncRepository _repository;

        public UserAccountsService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService,
            IAsyncRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _repository = repository;
        }

        public async Task<IEnumerable<AccountResponse>> GetAccountsList(AccountSpecificationsDTO specifications)
        {
            var accounts = await _userManager.Users.ToListAsync();

            List<AccountResponse> accountsList = new List<AccountResponse>();

            foreach (var account in accounts)
            {
                var role = (await _userManager.GetRolesAsync(account)).First();

                var accountResponse = new AccountResponse()
                {
                    Id = account.Id,
                    FullName = account.FullName,
                    Email = account.Email,
                    Role = role,
                    UserId = role == UserRoles.Doctor ? account.DoctorId : role == UserRoles.Patient ? account.PatientId : null
                };

                accountsList.Add(accountResponse);
            }

            return accountsList;
        }

        private async Task<ISpecification<ApplicationUser>> GetSpecification(AccountSpecificationsDTO specificationsDTO)
        {
            var builder = new SpecificationBuilder<ApplicationUser>();

            if (!string.IsNullOrEmpty(specificationsDTO.FullName))
                builder.With(x => x.FullName == specificationsDTO.FullName);

            if (!string.IsNullOrEmpty(specificationsDTO.Email))
                builder.With(x => x.Email == specificationsDTO.Email);

            if (!string.IsNullOrEmpty(specificationsDTO.Role))
            {
                var role = await _roleManager.FindByNameAsync(specificationsDTO.Role);

                if (role is not null)
                {
                    builder.With(x => x.UserRoles.First().RoleId == role.Id);
                }
            }

            switch (specificationsDTO.SortField)
            {
                case "Id":
                    builder.OrderBy(x => x.Id, specificationsDTO.SortDirection);
                    break;
                case "FullName":
                    builder.OrderBy(x => x.FullName, specificationsDTO.SortDirection);
                    break;
                case "Email":
                    builder.OrderBy(x => x.Email, specificationsDTO.SortDirection);
                    break;
            }

            builder.WithPagination(specificationsDTO.PageSize, specificationsDTO.PageNumber);

            return builder.Build();
        }

        public async Task CreateAccount(CreateAccountRequest user)
        {
            if (user.Role != UserRoles.Admin && user.Role != UserRoles.Receptionist)
            {
                throw new ArgumentException("You may create users only with roles of admin or receptionist.");
            }

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = string.Join(' ', user.Surname, user.Name, user.Patronymic),
                Email = user.Email,
                UserName = user.Email
            };

            var result = await _userManager.CreateAsync(applicationUser, user.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(applicationUser, user.Role);
            }
            else
            {
                throw new Exception($"Failed to create {user.Role}: {string.Join(", ", result.Errors)}");
            }
        }

        public async Task DeleteAccount(Guid accountId)
        {
            var user = await _userManager.FindByIdAsync(accountId.ToString());

            if (user is null)
            {
                throw new KeyNotFoundException("Account with passed id does not exist.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new ArgumentException($"Cannot delete user: {string.Join(", ", result.Errors)}");
            }
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginDTO login)
        {
            var user = await _userManager.FindByNameAsync(login.Email);

            if (user is null)
            {
                throw new KeyNotFoundException("User with such user name or email was not found.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password,
                isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var token = await _jwtService.CreateJwtToken(user);

                return new AuthenticationResponse
                {
                    UserId = user.DoctorId ?? user.PatientId ?? user.Id,
                    Role = roles.First(),
                    Token = token
                };
            }
            else
            {
                throw new ArgumentException("Cannot sign in user.");
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthenticationResponse> RegisterAsync(RegisterDTO register)
        {
            if (register.Role != UserRoles.Doctor && register.Role != UserRoles.Patient)
            {
                throw new ArgumentException("You may register only as doctor or patient.");
            }

            if (!await ValidateUserInfoMatchesWithRegisterCredentials(register))
            {
                throw new ArgumentException("Your credentials do not match with real user information.");
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = string.Join(' ', register.Surname, register.Name, register.Patronymic),
                Email = register.Email,
                UserName = register.Email
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, register.Role);
            }
            else
            {
                throw new ArgumentException($"Cannot create account for {register.Role}: {string.Join(", ", result.Errors)}");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var token = await _jwtService.CreateJwtToken(user);

            return new AuthenticationResponse
            {
                UserId = register.UserKey,
                Role = register.Role,
                Token = token
            };
        }

        private async Task<bool> ValidateUserInfoMatchesWithRegisterCredentials(RegisterDTO register)
        {
            bool result = false;

            switch (register.Role)
            {
                case UserRoles.Doctor:
                    var doctor = await _repository.GetByIdAsync<Doctor>(register.UserKey);

                    if (doctor is null) throw new KeyNotFoundException("Doctor with such Id does not exist.");

                    result = doctor.Name == register.Name &&
                             doctor.Surname == register.Surname &&
                             doctor.Patronymic == register.Patronymic &&
                             doctor.Email == register.Email;

                    break;
                case UserRoles.Patient:
                    var patient = await _repository.GetByIdAsync<Patient>(register.UserKey);

                    if (patient is null) throw new KeyNotFoundException("Patient with such Id does not exist.");

                    result = patient.Name == register.Name &&
                             patient.Surname == register.Surname &&
                             patient.Patronymic == register.Patronymic &&
                             patient.Email == register.Email;

                    break;
            }

            return result;
        }
    }
}
