using Grpc.Core;
using Tfplugin6;

namespace TfNet.PluginCore.Services;

internal partial class Terraform6ProviderService : Provider.ProviderBase
{
    public override async Task<CallFunction.Types.Response> CallFunction(CallFunction.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetFunctionProvider(request.Name) is { } provider
            ? await provider.CallFunctionAsync(request)
            : new()
            {
                Error = new FunctionError
                {
                    Text = $"Unknown function name '{request.Name}'."
                }
            };

    public override async Task<GetFunctions.Types.Response> GetFunctions(GetFunctions.Types.Request request, ServerCallContext context)
    {
        var response = new GetFunctions.Types.Response();

        var functions = await _resourceRegistry.GetFunctionsAsync().ToArrayAsync();
        foreach (var (key, function) in functions)
        {
            response.Functions.Add(key, function);
        }

        return response;
    }
}
