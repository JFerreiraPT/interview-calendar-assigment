using System;
namespace Interview_Calendar.Exceptions
{
	public class InvalidCredentialsException : Exception
	{
		public InvalidCredentialsException(string message)
			:base(message)
		{
		}
	}
}

