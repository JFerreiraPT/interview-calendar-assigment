using System;
using Interview_Calendar.Models.ValueObjects;

namespace Interview_Calendar.DTOs
{
	public class CandidateResponseDTO : UserDTO
	{
        public Interview Interview = default!;
        public string InterviewerId = default!;
    }
}

