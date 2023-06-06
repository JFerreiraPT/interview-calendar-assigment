using System;
namespace Interview_Calendar.Exceptions
{
	public class ResourceExistsException : Exception
	{
		public ResourceExistsException(string message)
			: base(message)
		{
		}
	}
}

