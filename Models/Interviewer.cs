using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Interview_Calendar.Models
{
	public class Interviewer : User
	{
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<DateOnly, SortedSet<int>> Availability = new Dictionary<DateOnly, SortedSet<int>>();
        public List<DateTime> Interviews = default!;
	}
}

