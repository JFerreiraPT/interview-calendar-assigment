using System;
using System.Text.Json.Serialization;

namespace Interview_Calendar.Models.ValueObjects
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserType
	{
        Interviewer,
        Candidate
	}
}

