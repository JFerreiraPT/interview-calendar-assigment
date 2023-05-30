using System;
using MongoDB.Bson;

namespace Interview_Calendar.Models
{
	public class BaseEntity : IBaseEntity
	{
		public BaseEntity()
		{
		}

        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

        public bool isActive { get; set; }
    }
}

