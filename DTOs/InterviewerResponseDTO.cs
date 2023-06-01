using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Interview_Calendar.DTOs
{
    public class InterviewerResponseDTO : UserDTO
	{

        [JsonPropertyName("availability")]
        public Dictionary<string, SortedSet<int>> Availability;
        [JsonPropertyName("interviews")]
        public List<DateTime> Interviews;
    }
}

