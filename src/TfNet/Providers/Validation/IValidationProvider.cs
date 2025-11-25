using TfNet.Models;

namespace TfNet.Providers.Validation;

public interface IValidationProvider<T>
{
    /// <summary>
    /// Validates the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<ValidationResult?> ValidateAsync(T value);
}
