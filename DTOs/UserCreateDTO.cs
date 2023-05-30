using System;
using System.ComponentModel.DataAnnotations;

namespace Interview_Calendar.DTOs
{
	public class UserCreateDTO : UserDTO
	{
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = default!;
    }
}

