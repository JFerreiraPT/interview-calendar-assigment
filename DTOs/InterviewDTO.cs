using System;
using System.ComponentModel.DataAnnotations;
using Interview_Calendar.Helpers;

namespace Interview_Calendar.DTOs
{
	public class InterviewDTO
	{
        [Required]
        [ValidateScheduleTime]             
        public DateTime date { get; set;}
    }
}

