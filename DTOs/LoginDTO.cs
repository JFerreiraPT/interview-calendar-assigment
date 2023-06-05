using System;
using System.ComponentModel.DataAnnotations;

namespace Interview_Calendar.DTOs
{
	public class LoginDTO
	{
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = default!;
    }
}

