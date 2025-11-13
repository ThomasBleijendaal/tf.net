using TfNet.Extensions;
using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Validation;

internal class ValidationProviderHost<T> : IValidationProviderHost
{
    private readonly IValidationProvider<T> _validator;
    private readonly IDynamicValueSerializer _serializer;

    public ValidationProviderHost(
        IValidationProvider<T> validator,
        IDynamicValueSerializer serializer)
    {
        _validator = validator;
        _serializer = serializer;
    }

    public async Task<Diagnostic[]> ValidateAsync(DynamicValue value)
    {
        var currentValue = _serializer.DeserializeDynamicValue<T>(value);

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
