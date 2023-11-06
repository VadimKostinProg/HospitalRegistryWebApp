using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalRegistry.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateJwtToken(ApplicationUser user)
        {
            // Get token expiration 
            var expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            // Configure claims for the payload
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Email),
            };

            var securityKey = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signinCredentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenGenerator =
                new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: expiration,
                    signingCredentials: signinCredentials
                );

            var tokenHandler =
                new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenGenerator);

            return token;
        }
    }
}
