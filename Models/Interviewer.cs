using System;
namespace Interview_Calendar.Models
{
	public class Interviewer : User
	{
        public Dictionary<DateOnly, SortedSet<int>> Availability = default!;
		public List<DateTime> Interviews = default!;
	}
}

