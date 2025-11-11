using TfNet.Extensions;
using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Validation;

internal class ValidationProviderHost<T> : Host<T>, IValidationProviderHost
{
    private readonly IValidationProvider<T> _validator;

    public ValidationProviderHost(
        IValidationProvider<T> validator,
        IDynamicValueSerializer serializer) : base(serializer)
    {
        _validator = validator;
    }

    public async Task<Diagnostic[]> ValidateAsync(DynamicValue value)
    {
        var currentValue = DeserializeDynamicValue(value);

        var validationResult = await _validator.ValidateAsync(currentValue);

        return validationResult?.ValidationErrors
            .SelectMany(
                v => v.Paths,
                (v, path) => new Diagnostic
                {
                    Detail = v.Message,
                    Severity = Diagnostic.Types.Severity.Error,
                    Attribute = path.Map()
                })
            .ToArray() ?? [];
    }
}
