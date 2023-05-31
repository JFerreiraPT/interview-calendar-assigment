using System;
namespace Interview_Calendar.DTOs
{
	public class AvailabilityRequestDTO
	{
        public DateOnly Date { get; set; }
        public int[]? TimeSlots { get; set; } = default!;
    }
}

