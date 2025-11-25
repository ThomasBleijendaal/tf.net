using TfNet.Providers.Validation;

namespace TfNet.Registry;

public interface IValidationRegisterer<T, TReturn>
{
    /// <summary>
    /// Add data annotation validation to validate incoming T's;
    /// </summary>
    /// <returns></returns>
    TReturn WithDataAnnotationValidation();

    /// <summary>
    /// Adds the given validator to validate incoming T's.
    /// </summary>
    /// <typeparam name="TValidator"></typeparam>
    /// <returns></returns>
    TReturn WithValidator<TValidator>() where TValidator : class, IValidationProvider<T>;
}
