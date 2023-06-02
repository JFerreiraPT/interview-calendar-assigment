using System;
using System.ComponentModel.DataAnnotations;
using Interview_Calendar.Models.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

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

