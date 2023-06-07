using System;
using System.Text.Json.Serialization;
using Interview_Calendar.Models.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace Interview_Calendar.Models
{
    //Not very dynamic but was the best way i could find to avoid Asbtract class initialization instead of the concrete one
    [BsonKnownTypes(typeof(Candidate), typeof(Interviewer))]
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

