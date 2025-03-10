using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Business.Validators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DateFormatValidator : ValidationAttribute
{
    private readonly string _dateFormat = "dd-MM-yyyy";

    public DateFormatValidator()
    {
        ErrorMessage = "Date format must be 'dd-MM-yyyy'.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is not string dateString)
        {
            return false;
        }

        return DateTime.TryParseExact(dateString, _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}