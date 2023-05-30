using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Services
{
	public class InterviewerService : IInterviewerService
	{
        public Task<Interviewer> PreCreateUserAsync(UserDTO dto)
        {
            //Transform DTO in entity with map helper + validations if needed
            throw new NotImplementedException();
        }

        public Task<Interviewer> CreateUserAsync(Interviewer entity)
        {
            //add to db context
            throw new NotImplementedException();
        }

        public Task<InterviewerResponseDTO> PostCreateUserAsync(Interviewer entity)
        {
            //mapp to response dto and handle post dependencies if any
            throw new NotImplementedException();
        }

    }
}

