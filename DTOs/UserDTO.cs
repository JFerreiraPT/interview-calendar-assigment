using System;
using System.ComponentModel.DataAnnotations;

namespace Interview_Calendar.DTOs
{
	public class UserDTO
	{
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}

