using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;
using MongoDB.Bson;

namespace Interview_Calendar.Services
{
	public interface IInterviewerService: IUserService<Interviewer, UserCreateDTO, InterviewerResponseDTO>
	{

        Task<bool> AddAvailability(string interviewerId, DateOnly date, int[] timeSlots);
        Dictionary<string, SortedSet<int>> GetInterviewersWithSchedulesBetweenDates(string interviewerObjectId, DateOnly startDate, DateOnly endDate);
        Task<bool> RemoveDayAvailability(string interviewerId, DateOnly date);

        Interviewer FindOrFail(string interviewerId);

        Task<bool> ScheduleInterview(string interviewerId, string candidateId);
        Task<InterviewerResponseDTO> GetInterviewer(string interviewerId);
    }
}

