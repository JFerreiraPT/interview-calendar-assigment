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
using Interview_Calendar.Models.ValueObjects;
using Interview_Calendar.Exceptions;

namespace Interview_Calendar.Services
{
    public class InterviewerService : IInterviewerService
    {
        private readonly IMongoCollection<Interviewer> _userCollection;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;

        private readonly AddUserService<Interviewer, UserCreateDTO, InterviewerResponseDTO> _addUserService;

        public InterviewerService(IOptions<UserDbConfiguration> userConfiguration, IMapper mapper,
            PasswordHasher hasher)
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

            var entity = await _addUserService.CreateUserAsync(user, UserType.Interviewer);

            return PostCreateUserAsync(entity);

        }

        public InterviewerResponseDTO PostCreateUserAsync(Interviewer entity)
        {
            //map to response dto and handle post dependencies if any

            return _addUserService.PostCreateUserAsync(entity);
        }

        public Interviewer FindOrFail(string interviewerId)
        {
            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(interviewerId)),
                Builders<Interviewer>.Filter.Eq("_id", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true)
            );

            var interviewer = _userCollection.Find(filter).FirstOrDefault();

            if(interviewer == null)
            {
                throw new NotFoundException("Interviwer Not Found");
            }

            return interviewer;
        }



        public async Task<bool> AddAvailability(string interviewerId, DateOnly date, int[] timeSlots)
        {


            //If date dont exist yet create new sorted list, otherwise update
            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(interviewerId)),
                Builders<Interviewer>.Filter.Eq("_id", typeof(Interviewer).Name),
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
                Builders<Interviewer>.Filter.Eq<string>("_id", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true)
            );

            var update = Builders<Interviewer>.Update.Unset($"Availability.{date}");

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;

        }


        public async Task<InterviewerResponseDTO> GetInterviewer(string interviewerId)
        {

            var interviewer = this.FindOrFail(interviewerId);


            return _mapper.Map<InterviewerResponseDTO>(interviewer);
        }

        private async Task<bool> IsInterviwerAvailable(string interviewerId, DateTime interviewTime)
        {

            string dateString = interviewTime.ToString("MM/dd/yyyy");
            int interviewHour = interviewTime.Hour;

            // Build the query to find the document with the specified date and the availability containing the interview hour
            var filter = Builders<Interviewer>.Filter.And(
                Builders<Interviewer>.Filter.Eq("_id", new ObjectId(interviewerId)),
                Builders<Interviewer>.Filter.Eq("_id", typeof(Interviewer).Name),
                Builders<Interviewer>.Filter.Eq("isActive", true),
                Builders<Interviewer>.Filter.Eq($"Availability.{dateString}", new BsonDocument("$in", new BsonArray { interviewHour }))
            );

            var result = await _userCollection.Find(filter).AnyAsync();

            return result;


        }

        private async Task<bool> CheckIfInterviwerAvailableAndRemove(Interviewer interviwer, DateTime interviewTime)
        {

            string dateString = interviewTime.ToString("MM/dd/yyyy");
            int interviewHour = interviewTime.Hour;

            if (interviwer.Availability.TryGetValue(dateString, out SortedSet<int> availableHours))
            {
                bool isAvailable = availableHours.Contains(interviewHour);
                if (isAvailable)
                {
                    // Remove the availability for the specified date and hour
                    availableHours.Remove(interviewHour);

                    // If no more available hours exist for the date, remove it from the dictionary
                    if (availableHours.Count == 0)
                    {
                        interviwer.Availability.Remove(dateString);
                    }

                    return true;
                }
            }

            return false;


        }

        public Dictionary<string, SortedSet<int>> GetInterviewersWithSchedulesBetweenDates(string interviewerId, DateOnly startDate, DateOnly endDate)
        {

            // The initial idea was to retrieve the availability list directly with one query.
            // However, filtering nested documents proved to be a complex task.
            // Since this is just a demo program to test MongoDB, I will filter it in memory.

            var interviewer = this.FindOrFail(interviewerId);

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

        public async Task<bool> ScheduleInterview(string interviewerId, string candidateId, DateTime date)
        {
            var interviewer = FindOrFail(interviewerId);

            //Check for availability
            if(!(await this.CheckIfInterviwerAvailableAndRemove(interviewer, date)))
            {
                throw new ValidationErrorException("Interviwer not available");
            }

            var interview = new Interview
            {
                date = date,
                CandidateId = candidateId
            };



            interviewer.Interviews ??= new List<Interview>();
            // Add the interview to the interviewer's Interviews list
            interviewer.Interviews.Add(interview);



            // Save the changes to the interviewer document in the database
            var updateResult = await _userCollection.ReplaceOneAsync(
                Builders<Interviewer>.Filter.Eq<ObjectId>("_id", ObjectId.Parse(interviewerId)),
                interviewer,
                new ReplaceOptions { IsUpsert = false });

            // Check if the update was successful
            return updateResult.ModifiedCount > 0;

        }
    }
}

