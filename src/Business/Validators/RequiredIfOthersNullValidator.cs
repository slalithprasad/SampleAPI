using System;
using System.ComponentModel.DataAnnotations;

namespace Business.Validators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequiredIfOthersNullValidator : ValidationAttribute
{
    private readonly string[] _otherPropertyNames;

    public RequiredIfOthersNullValidator(params string[] otherPropertyNames)
    {
        _otherPropertyNames = otherPropertyNames;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        string currentValue = value?.ToString() ?? string.Empty;

        if (!string.IsNullOrEmpty(currentValue))
        {
            return ValidationResult.Success!;
        }

        var instance = validationContext.ObjectInstance;
        bool othersAreNull = _otherPropertyNames.All(prop =>
        {
            var propertyInfo = instance.GetType().GetProperty(prop);
            var propertyValue = propertyInfo?.GetValue(instance, null);
            return propertyValue == null;
        });

        if (othersAreNull && value == null)
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required when {string.Join(", ", _otherPropertyNames)} are null.");
        }

        return ValidationResult.Success!;
    }
}
