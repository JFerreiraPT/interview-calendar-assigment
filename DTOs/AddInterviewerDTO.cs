using System;
using System.ComponentModel.DataAnnotations;

namespace Interview_Calendar.DTOs
{
	public class AddInterviewerDTO
	{
        [Required]
        public string interviewerId { get; set; } = default!;
    }
}

