using System;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Interview_Calendar.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IMongoCollection<Candidate> _userCollection;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;
        private readonly IInterviewerService _interviewerService;

        private readonly AddUserService<Candidate, UserCreateDTO, CandidateResponseDTO> _addUserService;

        public CandidateService(IOptions<UserDbConfiguration> userConfiguration, IMapper mapper, PasswordHasher hasher,
            IInterviewerService interviewerService)
        {
            _mapper = mapper;
            _passwordHasher = hasher;
            _interviewerService = interviewerService;
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

        public async Task<bool> AssignInterviewer(string id, AddInterviewerDTO interviwer)
        {
            //find candidate
            var filter = Builders<Candidate>.Filter.And(
                Builders<Candidate>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(id)),
                Builders<Candidate>.Filter.Eq("_t", typeof(Candidate).Name),
                Builders<Candidate>.Filter.Eq("isActive", true)
            );

            var candidate = _userCollection.Find(filter).FirstOrDefault();

            if (candidate == null)
            {
                throw new Exception();
            }

            if (_interviewerService.FindOrFail(interviwer.interviewerId) == null)
            {
                throw new Exception();
            }

            //Add
            candidate.InterviewerId = interviwer.interviewerId;

            var updateResult = await _userCollection.ReplaceOneAsync(
                filter,
                candidate,
                new ReplaceOptions { IsUpsert = true }
            );

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;


        }

        public async Task<CandidateResponseDTO> GetCandidate(string candidateId)
        {
            var filter = Builders<Candidate>.Filter.And(
                Builders<Candidate>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(candidateId)),
                Builders<Candidate>.Filter.Eq("_t", typeof(Candidate).Name),
                Builders<Candidate>.Filter.Eq("isActive", true)
            );

            var candidate = await _userCollection.FindAsync(filter);


            return _mapper.Map<CandidateResponseDTO>(candidate);
        }

        public Candidate FindOrFail(string candidateId)
        {
            var filter = Builders<Candidate>.Filter.And(
                Builders<Candidate>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(candidateId)),
                Builders<Candidate>.Filter.Eq("_t", typeof(Candidate).Name),
                Builders<Candidate>.Filter.Eq("isActive", true)
            );

            var candidate = _userCollection.Find(filter).FirstOrDefault();

            if(candidate == null)
            {
                throw new Exception("Not Found");
            }

            return candidate;
        }
    }
}

