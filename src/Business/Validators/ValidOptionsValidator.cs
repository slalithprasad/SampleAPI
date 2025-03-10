using System;
using System.ComponentModel.DataAnnotations;

namespace Business.Validators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidOptionsValidator : ValidationAttribute
{
    private readonly string[] _allowedValues;

    public ValidOptionsValidator(params string[] allowedValues)
    {
        _allowedValues = allowedValues;
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is string stringValue)
        {
            return Array.Exists(_allowedValues, val => string.Equals(val, stringValue, StringComparison.OrdinalIgnoreCase));
        }
        return false;
    }
}