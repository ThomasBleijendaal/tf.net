using TfNet.Registry;
using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Function;

internal class FunctionProviderHost<TRequest, TResponse> : IFunctionProviderHost
    where TRequest : new()
{
    private readonly FunctionRegistry _functionRegistry;
    private readonly IFunctionProvider<TRequest, TResponse> _functionProvider;
    private readonly IDynamicValueSerializer _serializer;

    public FunctionProviderHost(
        FunctionRegistry functionRegistry,
        IFunctionProvider<TRequest, TResponse> functionProvider,
        IDynamicValueSerializer serializer)
    {
        _functionRegistry = functionRegistry;
        _functionProvider = functionProvider;
        _serializer = serializer;
    }

    public async Task<CallFunction.Types.Response> CallFunctionAsync(CallFunction.Types.Request request)
    {
        var req = new TRequest();

        var setter = await _functionRegistry.GetFunctionRequestSetterAsync(request.Name);
        if (setter == null)
        {
            return new CallFunction.Types.Response
            {
                Error = new FunctionError { Text = "No setter for request" }
            };
        }

        setter.SetRequest(_serializer, request, req);

        var response = await _functionProvider.CallAsync(req);

        var result = _serializer.SerializeDynamicValue(response);

        return new CallFunction.Types.Response
        {
            Result = result
        };
    }
}
