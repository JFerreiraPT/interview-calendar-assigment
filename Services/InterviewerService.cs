using System;
using AutoMapper;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;
using MongoDB.Driver;

namespace Interview_Calendar.Services
{
	public class InterviewerService : IInterviewerService
	{
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMapper _mapper;

        public InterviewerService(IMongoCollection<User> userCollection, IMapper mapper)
        {
            _userCollection = userCollection;
            _mapper = mapper;
        }

        public Interviewer PreCreateUserAsync(UserDTO dto)
        {
            //Transform DTO in entity with map helper + validations if needed

            var interviwer = _mapper.Map<Interviewer>(dto);

            //hash password

            return interviwer;
        }

        public async Task<InterviewerResponseDTO> CreateUserAsync(UserDTO dto)
        {
            //add to db context
            var user = PreCreateUserAsync(dto);

            return PostCreateUserAsync(user);

        }

        public InterviewerResponseDTO PostCreateUserAsync(Interviewer entity)
        {
            //map to response dto and handle post dependencies if any
            throw new NotImplementedException();
        }

    }
}

