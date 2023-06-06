using System;
using Newtonsoft.Json;

namespace Interview_Calendar.Models
{
	public class Error
	{
		public int StatusCode { get; set; }
		public string Message { get; set; } = default!;
		public override string ToString() => JsonConvert.SerializeObject(this);

	}
}

