using System;
using System.ComponentModel.DataAnnotations;

namespace Interview_Calendar.Helpers
{
	public sealed class ValidateScheduleTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value is DateTime dateTime)
            {
                // Check if the time has hour, minutes, and seconds equal to zero
                if (dateTime.TimeOfDay.Minutes != 0 || dateTime.TimeOfDay.Seconds != 0)
                {
                    return new ValidationResult("Invalid interview time. The time should be exactly at hour " + dateTime.TimeOfDay.Hours + "00:00");
                }
            }

            return ValidationResult.Success;
        }
    }
}

