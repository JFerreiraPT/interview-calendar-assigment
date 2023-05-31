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
        private readonly IMongoCollection<Candidate> _userCollection;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;

        private readonly AddUserService<Candidate, UserCreateDTO, CandidateResponseDTO> _addUserService;

        public CandidateService(IOptions<UserDbConfiguration> userConfiguration, IMapper mapper, PasswordHasher hasher)
        {
            _mapper = mapper;
            _passwordHasher = hasher;
            var mongoClient = new MongoClient(userConfiguration.Value.ConnectionString);
            var userDatabase = mongoClient.GetDatabase(userConfiguration.Value.DatabaseName);
            _userCollection = userDatabase.GetCollection<Candidate>(userConfiguration.Value.UserCollectionName);


            _addUserService = new AddUserService<Candidate, UserCreateDTO, CandidateResponseDTO>(
                userDatabase.GetCollection<User>(userConfiguration.Value.UserCollectionName),
                _mapper,
                _passwordHasher);
        }


        public Candidate PreCreateUserAsync(UserCreateDTO dto)
        {
            
            return _addUserService.PreCreateUserAsync(dto);
            
        }
        public async Task<CandidateResponseDTO> CreateUserAsync(UserCreateDTO dto)
        {
            //add to db context
            var user = PreCreateUserAsync(dto);

            var entity = await _addUserService.CreateUserAsync(user);

            return PostCreateUserAsync(entity);            

        }

        public CandidateResponseDTO PostCreateUserAsync(Candidate entity)
        {
            return _addUserService.PostCreateUserAsync(entity);
        }


    }
}

