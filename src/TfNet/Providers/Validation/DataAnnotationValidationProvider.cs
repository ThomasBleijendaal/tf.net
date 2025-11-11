using System.ComponentModel.DataAnnotations;
using TfNet.Extensions;
using TfNet.Models;
using DataAnnotationsValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace TfNet.Providers.Validation;

internal class DataAnnotationValidationProvider<T> : IValidationProvider<T>
{
    public Task<Models.ValidationResult?> ValidateAsync(T value)
    {
        var results = new List<DataAnnotationsValidationResult>();

        if (value == null ||
            Validator.TryValidateObject(value, new ValidationContext(value), results, validateAllProperties: true))
        {
            return Task.FromResult(Models.ValidationResult.Success);
        }

        return Task.FromResult<Models.ValidationResult?>(new()
        {
            ValidationErrors = results
                .Select(x =>
                    new ValidationError(
                        x.ErrorMessage ?? "",
                        x.MemberNames.Select(n => new AttributePath(n.ToFirstLetterLower())).ToArray()))
                .ToList()
        });
    }
}
