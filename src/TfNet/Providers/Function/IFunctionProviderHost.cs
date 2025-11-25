using Tfplugin6;

namespace TfNet.Providers.Function;

internal interface IFunctionProviderHost
{
    Task<CallFunction.Types.Response> CallFunctionAsync(CallFunction.Types.Request request);
}
