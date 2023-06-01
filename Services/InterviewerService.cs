using System;
using AutoMapper;
using Interview_Calendar.Data;
using Interview_Calendar.DTOs;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Globalization;
using NodaTime.Text;

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



        public async Task<bool> AddAvailability(string interviewerId, DateOnly date, int[] timeSlots)
        {


            //If date dont exist yet create new sorted list, otherwise update
            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(interviewerId)),
                Builders<Interviewer>.Filter.Eq("_t", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true)
            );

            var update = Builders<Interviewer>.Update.Set($"Availability.{date}", new SortedSet<int>(timeSlots));

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveDayAvailability(string interviewerId, DateOnly date)
        {
            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(interviewerId)),
                Builders<Interviewer>.Filter.Eq<string>("_t", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true)
            );

            var update = Builders<Interviewer>.Update.Unset($"Availability.{date}");

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;

        }


        public async Task<InterviewerResponseDTO> GetInterviewer(string interviewerId)
        {
            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(interviewerId)),
                Builders<Interviewer>.Filter.Eq("_t", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true)
            );

            var interviewer = await _userCollection.FindAsync(filter);


            return _mapper.Map<InterviewerResponseDTO>(interviewer);
        }

        private async Task<bool> IsInterviwerAvailable(string interviewerId, DateTime interviewTime)
        {

            string dateString = interviewTime.ToString("MM/dd/yyyy");
            int interviewHour = interviewTime.Hour;

            // Build the query to find the document with the specified date and the availability containing the interview hour
            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq("_id", new ObjectId(interviewerId)),
                Builders<Interviewer>.Filter.Eq("_t", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true),
                Builders<Interviewer>.Filter.Eq($"Availability.{dateString}", new BsonDocument("$in", new BsonArray { interviewHour }))
            );

            var result = await _userCollection.Find(filter).AnyAsync();

            return result;


        }

        public Dictionary<string, SortedSet<int>> GetInterviewersWithSchedulesBetweenDates(string interviewerId, DateOnly startDate, DateOnly endDate)
        {

            // The initial idea was to retrieve the availability list directly with one query.
            // However, filtering nested documents proved to be a complex task.
            // Since this is just a demo program to test MongoDB, I will filter it in memory.

            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(interviewerId)),
                Builders<Interviewer>.Filter.Eq("_t", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true)
            );

            var interviewer = _userCollection.Find(filter).FirstOrDefault();

            if (interviewer == null)
            {
                // Handle interviewer not found
                return new Dictionary<string, SortedSet<int>>();
            }

            var result = new Dictionary<string, SortedSet<int>>();


            foreach (var key in interviewer.Availability.Keys)
            {
                if (DateOnly.Parse(key) >= startDate && DateOnly.Parse(key) <= endDate)
                {
                    result[key.ToString()] = new SortedSet<int>(interviewer.Availability[key]);
                }
            }

            return result;

        }


    }
}

