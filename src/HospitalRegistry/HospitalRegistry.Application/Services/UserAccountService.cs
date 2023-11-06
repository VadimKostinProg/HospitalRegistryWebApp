using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Microsoft.AspNetCore.Identity;

namespace HospitalRegistry.Application.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IAsyncRepository _repository;

        public UserAccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, IAsyncRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _repository = repository;
        }

        public async Task CreateUser(CreateUserDTO user)
        {
            if (user.Role != UserRoles.Admin && user.Role != UserRoles.Receptionist)
            {
                throw new ArgumentException("You may create users only with roles of admin or receptionist.");
            }

            var admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = string.Join(' ', user.Surname, user.Name, user.Patronymic),
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(admin, user.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, user.Role);
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
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, 
                isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
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
                UserId = register.UserId,
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
                    var doctor = await _repository.GetByIdAsync<Doctor>(register.UserId);

                    if (doctor is null) throw new KeyNotFoundException("Doctor with such Id does not exist.");

                    result = doctor.Name == register.Name &&
                             doctor.Surname == register.Surname &&
                             doctor.Patronymic == register.Patronymic &&
                             doctor.Email == register.Email;

                    break;
                case UserRoles.Patient:
                    var patient = await _repository.GetByIdAsync<Patient>(register.UserId);

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
