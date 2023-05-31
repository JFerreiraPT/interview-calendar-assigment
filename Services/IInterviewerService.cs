using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Services
{
	public interface IInterviewerService: IUserService<Interviewer, UserCreateDTO, InterviewerResponseDTO>
	{

        void AddAvailability(string interviewerId, DateOnly date, int[] timeSlots);
        void RemoveAvailability(string interviewerId, DateOnly date, int[] timeSlots);
        void UpdateAvailability(string interviewerId, DateOnly date, int[] timeSlots);
    }
}

