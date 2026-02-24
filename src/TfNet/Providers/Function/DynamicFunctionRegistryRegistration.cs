using TfNet.Schemas;

namespace TfNet.Providers.Function;

internal record DynamicFunctionRegistryRegistration(
    string ResourceName,
    string HandlerFunctionName,
    FunctionSignature FunctionSignature,
    IFunctionHandler Handler) : IFunctionRegistration;
