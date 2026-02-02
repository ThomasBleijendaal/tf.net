using Grpc.Core;
using Tfplugin6;

namespace TfNet.PluginCore.Services;

internal partial class Terraform6ProviderService : Provider.ProviderBase
{
    public override async Task<CallFunction.Types.Response> CallFunction(CallFunction.Types.Request request, ServerCallContext context)
        => (await _functionRegistry.GetFunctionProviderAsync(_serviceProvider, request.Name)) is { } provider
            ? await provider.CallFunctionAsync(request)
            : new()
            {
                Error = new FunctionError
                {
                    Text = $"Unknown function name '{request.Name}'."
                }
            };

    public override Task<GetFunctions.Types.Response> GetFunctions(GetFunctions.Types.Request request, ServerCallContext context)
    {
        var response = new GetFunctions.Types.Response();

        var functions = _functionRegistry.GetFunctions();
        foreach (var (key, function) in functions)
        {
            response.Functions.Add(key, function);
        }

        return Task.FromResult(response);
    }
}
