using System;
using System.ComponentModel.DataAnnotations;

namespace Interview_Calendar.DTOs
{
	public class InterviewDTO
	{
        [Required]
        public DateTime date;
    }
}

