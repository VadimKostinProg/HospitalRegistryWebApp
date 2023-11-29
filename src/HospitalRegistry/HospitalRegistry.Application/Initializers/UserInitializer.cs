using HospitalRegistry.Application.Constants;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HospitalRegistry.Application.Initializers
{
    internal class UserInitializer : IUserInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configaration;
        private readonly ILogger<UserInitializer> _logger;

        public UserInitializer(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration, ILogger<UserInitializer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configaration = configuration;
            _logger = logger;
        }

        public void Initialize()
        {
            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new ApplicationRole { Name = UserRoles.Admin }).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new ApplicationRole { Name = UserRoles.Receptionist }).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new ApplicationRole { Name = UserRoles.Doctor }).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new ApplicationRole { Name = UserRoles.Patient }).GetAwaiter().GetResult();
            }

            if (!_userManager.Users.Any())
            {
                var sysAdminEmail = _configaration["SysAdminCredentials:Email"]!;
                var password = _configaration["SysAdminCredentials:Password"]!;
                var fullName = _configaration["SysAdminCredentials:FullName"]!;

                var user = new ApplicationUser
                {
                    FullName = fullName,
                    UserName = sysAdminEmail,
                    Email = sysAdminEmail
                };

                var result = _userManager.CreateAsync(user, password).GetAwaiter().GetResult();

                if (result.Succeeded)
                    _userManager.AddToRoleAsync(user, UserRoles.Admin).GetAwaiter().GetResult();
                else
                    _logger.LogError("Error while creating sys admin: " + string.Join(", ", result.Errors.Select(x => x.Description)));
            }
        }
    }
}
