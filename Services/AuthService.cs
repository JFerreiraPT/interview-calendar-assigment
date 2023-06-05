using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Interview_Calendar.Models.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Interview_Calendar.Services
{
    public class AuthService : IAuthService
    {
        private const string TokenSecret = "57ab60f27d4192327fceda761b407a86061be862d48047e33e3c8120cf35ec13";
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

        private readonly IMongoCollection<User> _userCollection;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;


        public AuthService(IOptions<UserDbConfiguration> userConfiguration, IMapper mapper, PasswordHasher hasher)
        {
            _mapper = mapper;
            _passwordHasher = hasher;
            var mongoClient = new MongoClient(userConfiguration.Value.ConnectionString);
            var userDatabase = mongoClient.GetDatabase(userConfiguration.Value.DatabaseName);
            _userCollection = userDatabase.GetCollection<User>(userConfiguration.Value.UserCollectionName);
        }


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
                new("userType", user.UserType.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifetime),
                Issuer = "https://interview-calendar-api.com",
                Audience = "https://interview-calendar-ui.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret)), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }

        public string Login(LoginDTO loginDTO)
        {
            //Find user existence
            var user =_userCollection.Find(x => x.Email == loginDTO.Email).FirstOrDefault();
            if(user == null)
            {
                throw new Exception("User not found exception");
            }

            if(!_passwordHasher.verify(user.Password, loginDTO.Password))
            {
                throw new Exception("Invalid credentials");
            }

            return GenerateToken(user);
        }
    }
}

