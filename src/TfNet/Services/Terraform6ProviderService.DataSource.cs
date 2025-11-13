using Grpc.Core;
using Tfplugin6;

namespace TfNet.Services;

internal partial class Terraform6ProviderService : Provider.ProviderBase
{
    public override async Task<ValidateDataResourceConfig.Types.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Types.Request request, ServerCallContext context)
    {
        var response = new ValidateDataResourceConfig.Types.Response();

        // only validate when there is a validation provider registered
        if (_resourceRegistry.GetValidationProvider(request.TypeName) is { } provider)
        {
            response.Diagnostics.AddRange(await provider.ValidateAsync(request.Config));
        }
        return response;
    }

    public override async Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetDataSourceProvider(request.TypeName) is { } provider
            ? await provider.ReadDataSourceAsync(request)
            : new()
            {
                Diagnostics =
                {
                    new Diagnostic { Detail = $"Unknown type name '{request.TypeName}'." },
                }
            };
}
