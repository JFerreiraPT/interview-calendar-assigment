using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Interview_Calendar.Models
{
	public class Interviewer : User
	{
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string, SortedSet<int>> Availability = new Dictionary<string, SortedSet<int>>();
        public List<DateTime> Interviews = default!;
	}
}

