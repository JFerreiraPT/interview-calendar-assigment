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

        private readonly IInterviewerService _interviewerService;

        private readonly AddUserService<Candidate, UserCreateDTO, CandidateResponseDTO> _addUserService;

        public CandidateService(IOptions<UserDbConfiguration> userConfiguration,
            IMapper mapper,
            IInterviewerService interviewerService,
            AddUserService<Candidate, UserCreateDTO, CandidateResponseDTO> addUserService)
        {
            _mapper = mapper;
            _interviewerService = interviewerService;
            var mongoClient = new MongoClient(userConfiguration.Value.ConnectionString);
            var userDatabase = mongoClient.GetDatabase(userConfiguration.Value.DatabaseName);
            _userCollection = userDatabase.GetCollection<User>(userConfiguration.Value.UserCollectionName);


            _addUserService = addUserService;
        }


        public async Task<Candidate> PreCreateUserAsync(UserCreateDTO dto)
        {

            return await _addUserService.PreCreateUserAsync(dto);

        }
        public async Task<CandidateResponseDTO> CreateUserAsync(UserCreateDTO dto)
        {
            //add to db context
            var user = await PreCreateUserAsync(dto);

            var entity = await _addUserService.CreateUserAsync(user, UserType.Candidate);

            return PostCreateUserAsync(entity);

        }

        public CandidateResponseDTO PostCreateUserAsync(Candidate entity)
        {
            return _addUserService.PostCreateUserAsync(entity);
        }

        public async Task<bool> AssignInterviewer(string id, AddInterviewerDTO interviewer)
        {
            //find candidate
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(id)),
                Builders<User>.Filter.Eq("_t", typeof(Candidate).Name),
                Builders<User>.Filter.Eq("isActive", true)
            );

            var candidate = FindOrFail(id);

            if(candidate.InterviewerId != null)
            {
                throw new ValidationErrorException("there is already one interviwer assigned to candidate");
            }

            _interviewerService.FindOrFail(interviewer.interviewerId);
   

            //Add
            candidate.InterviewerId = interviewer.interviewerId;

            var updateResult = await _userCollection.ReplaceOneAsync(
                filter,
                candidate,
                new ReplaceOptions { IsUpsert = false }
            );

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;


        }

        public async Task<CandidateResponseDTO> GetCandidate(string candidateId)
        {

            var candidate = FindOrFail(candidateId);


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

            if (candidate.Interview != null)
            {
                throw new ValidationErrorException("Interview already schedulled");
            }

            //check if candidate has an interviewer associated
            if (candidate.InterviewerId == null)
            {
                throw new ValidationErrorException("No interviewer assigned yet");
            }

                        

            //Add to interviewer and remove availability
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

