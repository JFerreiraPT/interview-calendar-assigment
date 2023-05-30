using System;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Interview_Calendar.Services
{
	public class CandidateService : ICandidateService
	{
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;

        public CandidateService(IOptions<UserDbConfiguration> userConfiguration, IMapper mapper, PasswordHasher hasher)
        {
            _mapper = mapper;
            _passwordHasher = hasher;
            var mongoClient = new MongoClient(userConfiguration.Value.ConnectionString);
            var userDatabase = mongoClient.GetDatabase(userConfiguration.Value.DatabaseName);
            _userCollection = userDatabase.GetCollection<User>(userConfiguration.Value.UserCollectionName);
        }


        public Candidate PreCreateUserAsync(UserCreateDTO dto)
        {
            //Transform DTO in entity with map helper + validations if needed

            //Validate if there is any user with this email
            var userExists = _userCollection.Find(x => x.Email == dto.Email).Any();

            if (userExists)
            {
                throw new Exception();
            }


            //hash password
            dto.Password = _passwordHasher.Hash(dto.Password);

            var candidate = _mapper.Map<Candidate>(dto);

            return candidate;
        }
        public async Task<CandidateResponseDTO> CreateUserAsync(UserCreateDTO dto)
        {
            //add to db context
            var user = PreCreateUserAsync(dto);


            try
            {
                await _userCollection.InsertOneAsync(user);

                return PostCreateUserAsync(user);
            }
            catch (Exception ex)
            {
                //Todo:Throw exception
                throw new Exception();
            }
        }

        public CandidateResponseDTO PostCreateUserAsync(Candidate entity)
        {
            return _mapper.Map<CandidateResponseDTO>(entity);
        }


    }
}

