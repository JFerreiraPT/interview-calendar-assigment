using System;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
using Interview_Calendar.Exceptions;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Interview_Calendar.Models.ValueObjects;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

//this task will help on common user create tasks

namespace Interview_Calendar.Services
{
    public class AddUserService<T, DI, DO>
        where T : User
        where DI : UserCreateDTO
        where DO : UserDTO
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;

        public AddUserService(IOptions<UserDbConfiguration> userConfiguration,
            IMapper mapper,
            PasswordHasher passwordHasher)
        {
            
            _mapper = mapper;
            _passwordHasher = passwordHasher;

            var mongoClient = new MongoClient(userConfiguration.Value.ConnectionString);
            var userDatabase = mongoClient.GetDatabase(userConfiguration.Value.DatabaseName);
            _userCollection = userDatabase.GetCollection<User>(userConfiguration.Value.UserCollectionName);
        }

        public async Task<T> PreCreateUserAsync(DI dto)
        {
            //Validate if there is any user with this email
            var userExists = await _userCollection.Find(x => x.Email == dto.Email).AnyAsync();

            if (userExists)
            {
                throw new ResourceExistsException($"User with email {dto.Email} already exists");
            }

            //hash password
            dto.Password = _passwordHasher.Hash(dto.Password);

            var candidate = _mapper.Map<T>(dto);


            return candidate;
        }
        public async Task<T> CreateUserAsync(T user, UserType type)
        {

            user.UserType = type;
            await _userCollection.InsertOneAsync(user);

            return user;

        }

        public DO PostCreateUserAsync(T entity)
        {
            return _mapper.Map<DO>(entity);
        }


    }
}

