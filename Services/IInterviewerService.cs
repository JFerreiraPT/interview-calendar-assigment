using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Services
{
	public interface IInterviewerService: IUserService<Interviewer, UserCreateDTO, InterviewerResponseDTO>
	{
	}
}

