using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Interview_Calendar.Models;
using Interview_Calendar.Models.ValueObjects;
using Microsoft.IdentityModel.Tokens;

namespace Interview_Calendar.Services
{
    public class AuthService : IAuthService
    {
        private const string TokenSecret = "57ab60f27d4192327fceda761b407a86061be862d48047e33e3c8120cf35ec13";
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);


        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Name),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("userid", user.Id.ToString()),
                //Todo: should be improved
                new("role", user.UserType.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifetime),
                Issuer = "https://testapi.com",
                Audience = "https://testAudience.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret)), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }
    }
}

