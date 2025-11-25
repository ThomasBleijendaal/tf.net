using Tfplugin6;

namespace TfNet.Providers.Validation;

internal interface IValidationProviderHost
{
    Task<Diagnostic[]> ValidateAsync(DynamicValue value);
}
