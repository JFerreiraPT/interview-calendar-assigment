using System;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;

namespace Interview_Calendar.Services
{
	public class InterviewerService : IInterviewerService
	{
        private readonly IMongoCollection<Interviewer> _userCollection;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;

        private readonly AddUserService<Interviewer, UserCreateDTO, InterviewerResponseDTO> _addUserService;

        public InterviewerService(IOptions<UserDbConfiguration> userConfiguration, IMapper mapper, PasswordHasher hasher)
        {
            _mapper = mapper;
            _passwordHasher = hasher;
            var mongoClient = new MongoClient(userConfiguration.Value.ConnectionString);
            var userDatabase = mongoClient.GetDatabase(userConfiguration.Value.DatabaseName);
            _userCollection = userDatabase.GetCollection<Interviewer>(userConfiguration.Value.UserCollectionName);

            _addUserService = new AddUserService<Interviewer, UserCreateDTO, InterviewerResponseDTO>(
                userDatabase.GetCollection<User>(userConfiguration.Value.UserCollectionName),
                _mapper,
                _passwordHasher);
        }

        public Interviewer PreCreateUserAsync(UserCreateDTO dto)
        {
            //Transform DTO in entity with map helper + validations if needed

            return _addUserService.PreCreateUserAsync(dto);
        }

        public async Task<InterviewerResponseDTO> CreateUserAsync(UserCreateDTO dto)
        {

            var user = PreCreateUserAsync(dto);

            var entity = await _addUserService.CreateUserAsync(user);

            return PostCreateUserAsync(entity);

        }

        public InterviewerResponseDTO PostCreateUserAsync(Interviewer entity)
        {
            //map to response dto and handle post dependencies if any

            return _addUserService.PostCreateUserAsync(entity);
        }



        public void AddAvailability(string interviewerId, DateOnly date, int[] timeSlots)
        {
            var filter = Builders<Interviewer>.Filter.Eq("_id", interviewerId);
            var update = Builders<Interviewer>.Update.AddToSetEach(x => x.Availability[date], timeSlots);

            _userCollection.UpdateOne(filter, update);
        }

        public void RemoveAvailability(string interviewerId, DateOnly date, int[] timeSlots)
        {
            var filter = Builders<Interviewer>.Filter.Eq("_id", interviewerId);
            var update = Builders<Interviewer>.Update.PullAll(x => x.Availability[date], timeSlots);

            _userCollection.UpdateOne(filter, update);
        }

        public void UpdateAvailability(string interviewerId, DateOnly date, int[] timeSlots)
        {
            var filter = Builders<Interviewer>.Filter.Eq("_id", interviewerId);
            var update = Builders<Interviewer>.Update.Set(x => x.Availability[date], new SortedSet<int>(timeSlots));

            _userCollection.UpdateOne(filter, update);
        }


    }
}

