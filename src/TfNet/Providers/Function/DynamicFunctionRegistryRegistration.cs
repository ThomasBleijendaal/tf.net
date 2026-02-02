using TfNet.Schemas;

namespace TfNet.Providers.Function;

internal record DynamicFunctionRegistryRegistration(string ResourceName, string HandlerFunctionName, IFunctionHandler Handler) : IFunctionRegistration;
