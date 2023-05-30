using System;
using AutoMapper;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Helpers
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Interviewer, UserDTO>().ReverseMap();
			CreateMap<Interviewer, InterviewerResponseDTO>().ReverseMap();
		}
	}
}

