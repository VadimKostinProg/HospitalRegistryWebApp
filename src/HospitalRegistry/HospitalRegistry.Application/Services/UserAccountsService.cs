using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Application.Services
{
    public class UserAccountsService : IUserAccountsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IAsyncRepository _repository;
        private readonly ISpecificationsService _specificationsService;

        public UserAccountsService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IAsyncRepository repository,
            ISpecificationsService specificationsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _repository = repository;
            _specificationsService = specificationsService;
        }

        public async Task<IEnumerable<AccountResponse>> GetAccountsList(Specifications specifications)
        {
            var query = _userManager.Users;

            query = _specificationsService.ApplySpecifications(query, specifications);

            var accounts = await query.ToListAsync();

            List<AccountResponse> accountsList = new List<AccountResponse>();

            foreach (var account in accounts)
            {
                var role = (await _userManager.GetRolesAsync(account)).First();

                string fullName = string.Empty;
                Guid? userId = null;

                switch (role)
                {
                    case UserRoles.Admin:
                    case UserRoles.Receptionist:
                        fullName = account.FullName;
                        break;
                    case UserRoles.Doctor:
                        var doctor = account.Doctor;
                        fullName = string.Join(' ', doctor.Surname, doctor.Name, doctor.Patronymic);
                        userId = doctor.Id;
                        break;
                    case UserRoles.Patient:
                        var patient = account.Doctor;
                        fullName = string.Join(' ', patient.Surname, patient.Name, patient.Patronymic);
                        userId = patient.Id;
                        break;
                }

                var accountResponse = new AccountResponse()
                {
                    Id = account.Id,
                    FullName = fullName,
                    Email = account.Email,
                    Role = role,
                    UserId = userId,
                };

                accountsList.Add(accountResponse);
            }

            return accountsList;
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
