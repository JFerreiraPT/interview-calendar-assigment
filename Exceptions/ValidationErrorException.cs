using System;
namespace Interview_Calendar.Exceptions
{
	public class ValidationErrorException : Exception
	{
		public ValidationErrorException(string message)
			:base(message)
		{
		}
	}
}

