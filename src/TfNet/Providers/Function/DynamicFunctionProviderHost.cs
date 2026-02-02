using TfNet.Registry;
using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Function;

internal class DynamicFunctionProviderHost : IFunctionProviderHost
{
    private readonly ResourceRegistry _resourceRegistry;
    private readonly DynamicFunctionRegistryRegistration _functionRegistration;
    private readonly IDynamicValueSerializer _serializer;

    public DynamicFunctionProviderHost(
        ResourceRegistry resourceRegistry,
        DynamicFunctionRegistryRegistration functionRegistration,
        IDynamicValueSerializer serializer)
    {
        _resourceRegistry = resourceRegistry;
        _functionRegistration = functionRegistration;
        _serializer = serializer;
    }

    public async Task<CallFunction.Types.Response> CallFunctionAsync(CallFunction.Types.Request request)
    {
        var req = new Dictionary<string, object>();

        var setter = await _resourceRegistry.GetFunctionRequestSetterAsync(_functionRegistration.ResourceName);
        if (setter == null)
        {
            return new CallFunction.Types.Response
            {
                Error = new FunctionError { Text = "No setter for request" }
            };
        }

        setter.SetRequest(_serializer, request, req);

        var response = await _functionRegistration.Handler.HandleFunctionCallAsync(_functionRegistration.HandlerFunctionName, req);

        var result = _serializer.SerializeDynamicValue(response);

        return new CallFunction.Types.Response
        {
            Result = result
        };
    }
}
