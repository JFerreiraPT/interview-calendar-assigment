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

        public async Task<InterviewerResponseDTO> CreateUserAsync(UserDTO dto)
        {
            //add to db context
            var user = await PreCreateUserAsync(dto);

            return await PostCreateUserAsync(user);

        }

        public Task<InterviewerResponseDTO> PostCreateUserAsync(Interviewer entity)
        {
            //map to response dto and handle post dependencies if any
            throw new NotImplementedException();
        }

    }
}

