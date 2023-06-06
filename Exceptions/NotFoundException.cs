using System;
namespace Interview_Calendar.Exceptions
{
	public class NotFoundException : Exception
	{
        public NotFoundException(string message)
            : base(message)
        {
            
        }

    }
}

