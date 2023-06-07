using System;
using System.Text.Json.Serialization;
using Interview_Calendar.Models.ValueObjects;
using Newtonsoft.Json;

namespace Interview_Calendar.DTOs
{
    public class InterviewerResponseDTO : UserDTO
	{

        public Dictionary<string, SortedSet<int>> Availability = new Dictionary<string, SortedSet<int>>();
        public List<Interview> Interviews = new List<Interview>();
    }
}

