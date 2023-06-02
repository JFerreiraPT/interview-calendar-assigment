using System;
using Interview_Calendar.Models.ValueObjects;

namespace Interview_Calendar.Models
{
	public class Candidate : User
	{
        public Interview Interview = default!;
        public string InterviewerId = default!;
    }
}

