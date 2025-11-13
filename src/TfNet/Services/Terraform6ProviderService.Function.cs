using Grpc.Core;
using Tfplugin6;

namespace TfNet.Services;

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

    public override Task<GetFunctions.Types.Response> GetFunctions(GetFunctions.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.GetFunctions.Types.Response());
    }
}
