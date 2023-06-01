using System;
using System.ComponentModel.DataAnnotations;

namespace Interview_Calendar.Helpers
{
    public sealed class ValidateTimeSlotsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int[] timeSlots)
            {
                foreach (int timeSlot in timeSlots)
                {
                    if (timeSlot < 9 || timeSlot > 18)
                    {
                        return new ValidationResult("TimeSlots array contains elements outside the valid range of 9 to 18.");
                    }
                }
            }
            else if(value != null)
            {
                return new ValidationResult("Invalid type. Expected an int[] for TimeSlots array.");
            }

            return ValidationResult.Success;
        }
    }
}

