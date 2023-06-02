using System;
namespace Interview_Calendar.Models.ValueObjects
{
	public class Interview
	{
		public DateTime date;
		public string? InterviewerId = default!;
		public string? CandidateId = default!;
	}
}

