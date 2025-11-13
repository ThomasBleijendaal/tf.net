using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Function;

internal class FunctionProviderHost<TRequest, TResponse> : IFunctionProviderHost
{
    private readonly IDynamicValueSerializer _serializer;

    public FunctionProviderHost(
        IFunctionProvider<TRequest, TResponse> functionProvider,
        IDynamicValueSerializer serializer)
    {
        _serializer = serializer;
    }

    public async Task<CallFunction.Types.Response> CallFunctionAsync(CallFunction.Types.Request request)
    {
        return new CallFunction.Types.Response();

        // var input = _serializer.DeserializeDynamicValue<TRequest>(request.Arguments)
    }
}
