using Grpc.Core;
using Tfplugin6;
using static Tfplugin6.Provider;

namespace TfNet.Proxy.Services;

internal class Terraform6ProviderServices : ProviderBase
{
    private readonly ProviderClient _providerClient;

    public Terraform6ProviderServices(
        ProviderClient providerClient)
    {
        _providerClient = providerClient;
    }

    public override async Task<ConfigureProvider.Types.Response> ConfigureProvider(ConfigureProvider.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.ConfigureProviderAsync(request);
    }

    public override async Task<ValidateProviderConfig.Types.Response> ValidateProviderConfig(ValidateProviderConfig.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.ValidateProviderConfigAsync(request);
    }

    public override async Task<GetProviderSchema.Types.Response> GetProviderSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.GetProviderSchemaAsync(request);
    }

    public override async Task<GetResourceIdentitySchemas.Types.Response> GetResourceIdentitySchemas(GetResourceIdentitySchemas.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.GetResourceIdentitySchemasAsync(request);
    }

    public override async Task<ValidateResourceConfig.Types.Response> ValidateResourceConfig(ValidateResourceConfig.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.ValidateResourceConfigAsync(request);
    }

    public override async Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.PlanResourceChangeAsync(request);
    }

    public override async Task<CallFunction.Types.Response> CallFunction(CallFunction.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.CallFunctionAsync(request);
    }

    public override async Task<GetFunctions.Types.Response> GetFunctions(GetFunctions.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.GetFunctionsAsync(request);
    }

    public override async Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.ReadDataSourceAsync(request);
    }

    public override async Task<ValidateDataResourceConfig.Types.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Types.Request request, ServerCallContext context)
    {
        return await _providerClient.ValidateDataResourceConfigAsync(request);
    }
}
