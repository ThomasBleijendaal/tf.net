using Grpc.Core;
using Tfplugin6;

namespace TfNet.PluginCore.Services;

internal partial class Terraform6ProviderService : Provider.ProviderBase
{
    public override async Task<ValidateEphemeralResourceConfig.Types.Response> ValidateEphemeralResourceConfig(ValidateEphemeralResourceConfig.Types.Request request, ServerCallContext context)
    {
        var response = new ValidateEphemeralResourceConfig.Types.Response();

        // only validate when there is a validation provider registered
        if (_resourceRegistry.GetValidationProvider(_serviceProvider, request.TypeName) is { } provider)
        {
            response.Diagnostics.AddRange(await provider.ValidateAsync(request.Config));
        }

        return response;
    }

    public override async Task<OpenEphemeralResource.Types.Response> OpenEphemeralResource(OpenEphemeralResource.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetEphemeralResourceProvider(_serviceProvider, request.TypeName) is { } provider
            ? await provider.OpenAsync(request)
            : new()
            {
                Diagnostics =
                {
                new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };

    public override async Task<RenewEphemeralResource.Types.Response> RenewEphemeralResource(RenewEphemeralResource.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetEphemeralResourceProvider(_serviceProvider, request.TypeName) is { } provider
            ? await provider.RenewAsync(request)
            : new()
            {
                Diagnostics =
                {
                new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };

    public override async Task<CloseEphemeralResource.Types.Response> CloseEphemeralResource(CloseEphemeralResource.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetEphemeralResourceProvider(_serviceProvider, request.TypeName) is { } provider
            ? await provider.CloseAsync(request)
            : new()
            {
                Diagnostics =
                {
                new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };
}
