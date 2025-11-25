using FluentValidation;
using TfNet.Extensions;
using TfNet.Models;
using TfNet.Providers.Validation;

namespace TfNet.SampleCore;

public class FluentBaseValidator<T> : AbstractValidator<T>, IValidationProvider<T>
{
    Task<ValidationResult?> IValidationProvider<T>.ValidateAsync(T value)
    {
        var result = Validate(value);
        if (result == null)
        {
            return Task.FromResult(ValidationResult.Success);
        }

        return Task.FromResult(new ValidationResult
        {
            ValidationErrors = result.Errors
                .Select(x => new ValidationError(x.ErrorMessage, [new(x.PropertyName.ToFirstLetterLower())]))
                .ToList()
        })!;
    }
}

