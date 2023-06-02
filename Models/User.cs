using System;
using System.Text.Json.Serialization;
using Interview_Calendar.Models.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace Interview_Calendar.Models
{
	public abstract class User : BaseEntity
	{
        [BsonRequired]
        public string Name { get; set; } = default!;
        [BsonRequired]
        public string Email { get; set; } = default!;
        [BsonRequired]
        public string Password { get; set; } = default!;
        [BsonRequired]
        public UserType UserType { get; set; }

    }
}

