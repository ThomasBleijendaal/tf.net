using Grpc.Core;
using Tfplugin6;

namespace TfNet.Services;

internal partial class Terraform6ProviderService : Provider.ProviderBase
{
    public override async Task<ValidateResourceConfig.Types.Response> ValidateResourceConfig(ValidateResourceConfig.Types.Request request, ServerCallContext context)
    {
        var response = new ValidateResourceConfig.Types.Response();

        // only validate when there is a validation provider registered
        if (_resourceRegistry.GetValidationProvider(request.TypeName) is { } provider)
        {
            response.Diagnostics.AddRange(await provider.ValidateAsync(request.Config));
        }
        return response;
    }

    public override async Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetResourceProvider(request.TypeName) is { } provider
            ? await provider.PlanResourceChangeAsync(request)
            : new()
            {
                Diagnostics =
                {
                    new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };

    public override async Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetResourceProvider(request.TypeName) is { } provider
            ? await provider.ApplyResourceChangeAsync(request)
            : new()
            {
                Diagnostics =
                {
                    new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };

    public override async Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetResourceProvider(request.TypeName) is { } provider
            ? await provider.UpgradeResourceStateAsync(request)
            : new()
            {
                Diagnostics =
                {
                    new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };

    public override async Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetResourceProvider(request.TypeName) is { } provider
            ? await provider.ReadResourceAsync(request)
            : new()
            {
                Diagnostics =
                {
                    new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };

    public override async Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetResourceProvider(request.TypeName) is { } provider
            ? await provider.ImportResourceStateAsync(request)
            : new()
            {
                Diagnostics =
                {
                    new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };
}
