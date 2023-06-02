using System;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
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

        public AddUserService(IMongoCollection<User> userCollection, IMapper mapper, PasswordHasher passwordHasher)
        {
            _userCollection = userCollection;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public T PreCreateUserAsync(DI dto)
        {
            //Validate if there is any user with this email
            var userExists = _userCollection.Find(x => x.Email == dto.Email).Any();

            if (userExists)
            {
                throw new Exception();
            }

            //hash password
            dto.Password = _passwordHasher.Hash(dto.Password);

            var candidate = _mapper.Map<T>(dto);


            return candidate;
        }
        public async Task<T> CreateUserAsync(T user, UserType type)
        {

            try
            {
                user.UserType = type;
                await _userCollection.InsertOneAsync(user);

                return user;

            }
            catch (Exception ex)
            {
                //Todo:Throw exception
                throw new Exception();
            }
        }

        public DO PostCreateUserAsync(T entity)
        {
            return _mapper.Map<DO>(entity);
        }


    }
}

