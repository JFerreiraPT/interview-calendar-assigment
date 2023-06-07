using System;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
using Interview_Calendar.Exceptions;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Interview_Calendar.Models.ValueObjects;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Interview_Calendar.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IMongoCollection<User> _userCollection;
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
            _userCollection = userDatabase.GetCollection<User>(userConfiguration.Value.UserCollectionName);


            _addUserService = new AddUserService<Candidate, UserCreateDTO, CandidateResponseDTO>(
                _userCollection,
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

            var entity = await _addUserService.CreateUserAsync(user, UserType.Candidate);

            return PostCreateUserAsync(entity);

        }

        public CandidateResponseDTO PostCreateUserAsync(Candidate entity)
        {
            return _addUserService.PostCreateUserAsync(entity);
        }

        public async Task<bool> AssignInterviewer(string id, AddInterviewerDTO interviwer)
        {
            //find candidate
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(id)),
                Builders<User>.Filter.Eq("_t", typeof(Candidate).Name),
                Builders<User>.Filter.Eq("isActive", true)
            );

            var candidate = FindOrFail(id);

            _interviewerService.FindOrFail(interviwer.interviewerId);
   

            //Add
            candidate.InterviewerId = interviwer.interviewerId;

            var updateResult = await _userCollection.ReplaceOneAsync(
                filter,
                candidate,
                new ReplaceOptions { IsUpsert = false }
            );

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;


        }

        public async Task<CandidateResponseDTO> GetCandidate(string candidateId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(candidateId)),
                Builders<User>.Filter.Eq("_t", typeof(Candidate).Name),
                Builders<User>.Filter.Eq("isActive", true)
            );

            var candidate = await _userCollection.FindAsync(filter);


            return _mapper.Map<CandidateResponseDTO>(candidate);
        }

        public Candidate FindOrFail(string candidateId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(candidateId)),
                Builders<User>.Filter.Eq("_t", typeof(Candidate).Name),
                Builders<User>.Filter.Eq("isActive", true)
            );


            var candidate = (Candidate)_userCollection.Find(filter).FirstOrDefault();

            if(candidate == null)
            {
                throw new NotFoundException("Candidate Not Found");
            }

            return candidate;
        }

        public async Task<bool> ScheduleInterview(string candidateId, DateTime date)
        {
            //check if interviewer exists
            var candidate = FindOrFail(candidateId);

            //check if candidate has an interviewer associated
            if(candidate.InterviewerId == null)
            {
                throw new ValidationErrorException("Not interviewer assigned yet");
            }

            

            //Add to interviwer and remove availability
            await _interviewerService.ScheduleInterview(candidate.InterviewerId, candidateId, date);

            

            var interview = new Interview
            {
                date = date,
                InterviewerId = candidate.InterviewerId
            };


            // Add the interview to the interviewer's Interviews list
            candidate.Interview = interview;

            // Save the changes to the interviewer document in the database
            var updateResult = await _userCollection.ReplaceOneAsync(
                Builders<User>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(candidateId)),
                candidate);

            // Check if the update was successful
            return updateResult.ModifiedCount > 0;

        }
    }
}

