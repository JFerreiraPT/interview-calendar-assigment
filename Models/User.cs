using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Interview_Calendar.Models
{
	public class User : BaseEntity
	{
        [BsonRequired]
        public string Name { get; set; } = default!;
        [BsonRequired]
        public string Email { get; set; } = default!;
        [BsonRequired]
        public string Password { get; set; } = default!;

    }
}

