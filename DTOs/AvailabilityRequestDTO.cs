using System;
using Interview_Calendar.Helpers;

namespace Interview_Calendar.DTOs
{
	public class AvailabilityRequestDTO
	{
        public DateOnly Date { get; set; }
        [ValidateTimeSlots]
        public int[]? TimeSlots { get; set; } = default!;
    }
}

