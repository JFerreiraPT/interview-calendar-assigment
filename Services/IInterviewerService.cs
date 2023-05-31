using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Services
{
	public interface IInterviewerService: IUserService<Interviewer, UserCreateDTO, InterviewerResponseDTO>
	{

        Task<bool> AddAvailability(string interviewerId, DateOnly date, int[] timeSlots);
        Task<bool> RemoveDayAvailability(string interviewerId, DateOnly date);
    }
}

