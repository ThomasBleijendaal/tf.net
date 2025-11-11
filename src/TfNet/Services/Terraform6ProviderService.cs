using AsyncPlinq;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TfNet.ProviderConfig;
using TfNet.Providers.Data;
using TfNet.Registry;
using Tfplugin6;

namespace TfNet.Services;

internal partial class Terraform6ProviderService : Provider.ProviderBase
{
    private readonly ILogger<Terraform6ProviderService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ResourceRegistry _resourceRegistry;
    private readonly IServiceProvider _serviceProvider;
    private readonly ProviderConfigurationRegistry? _providerConfiguration;

    public Terraform6ProviderService(
        ILogger<Terraform6ProviderService> logger,
        IHostApplicationLifetime lifetime,
        ResourceRegistry resourceRegistry,
        IServiceProvider serviceProvider,
        ProviderConfigurationRegistry? providerConfiguration = null)
    {
        _logger = logger;
        _lifetime = lifetime;
        _resourceRegistry = resourceRegistry;
        _serviceProvider = serviceProvider;
        _providerConfiguration = providerConfiguration;
    }

    public override Task<GetResourceIdentitySchemas.Types.Response> GetResourceIdentitySchemas(GetResourceIdentitySchemas.Types.Request request, ServerCallContext context)
    {
        var res = new GetResourceIdentitySchemas.Types.Response();

        return Task.FromResult(res);
    }

    public override Task<ValidateDataResourceConfig.Types.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Types.Request request, ServerCallContext context)
    {
        var res = new ValidateDataResourceConfig.Types.Response();

        return Task.FromResult(res);
    }

    public override Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.DataTypes.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ReadDataSource.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(DataSourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ReadDataSource.Types.Response>)providerHostType.GetMethod(nameof(DataSourceProviderHost<object>.ReadDataSourceAsync))!
            .Invoke(provider, new[] { request })!;
    }

    // unimplemented stubs

    public override Task<CallFunction.Types.Response> CallFunction(CallFunction.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.CallFunction.Types.Response());
    }

    public override Task<CloseEphemeralResource.Types.Response> CloseEphemeralResource(CloseEphemeralResource.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.CloseEphemeralResource.Types.Response());
    }

    public override Task<ConfigureStateStore.Types.Response> ConfigureStateStore(ConfigureStateStore.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.ConfigureStateStore.Types.Response());
    }

    public override Task<DeleteState.Types.Response> DeleteState(DeleteState.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.DeleteState.Types.Response());
    }

    public override Task<GenerateResourceConfig.Types.Response> GenerateResourceConfig(GenerateResourceConfig.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.GenerateResourceConfig.Types.Response());
    }

    public override Task<GetFunctions.Types.Response> GetFunctions(GetFunctions.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.GetFunctions.Types.Response());
    }

    public override Task<GetMetadata.Types.Response> GetMetadata(GetMetadata.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.GetMetadata.Types.Response());
    }

    public override Task<GetStates.Types.Response> GetStates(GetStates.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.GetStates.Types.Response());
    }

    public override Task InvokeAction(InvokeAction.Types.Request request, IServerStreamWriter<InvokeAction.Types.Event> responseStream, ServerCallContext context)
    {
        return Task.CompletedTask;
    }

    public override Task ListResource(ListResource.Types.Request request, IServerStreamWriter<ListResource.Types.Event> responseStream, ServerCallContext context)
    {
        return Task.CompletedTask;
    }

    public override Task<LockState.Types.Response> LockState(LockState.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.LockState.Types.Response());
    }

    public override Task<MoveResourceState.Types.Response> MoveResourceState(MoveResourceState.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.MoveResourceState.Types.Response());
    }

    public override Task<OpenEphemeralResource.Types.Response> OpenEphemeralResource(OpenEphemeralResource.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.OpenEphemeralResource.Types.Response());
    }

    public override Task<PlanAction.Types.Response> PlanAction(PlanAction.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.PlanAction.Types.Response());
    }

    public override Task ReadStateBytes(ReadStateBytes.Types.Request request, IServerStreamWriter<ReadStateBytes.Types.Response> responseStream, ServerCallContext context)
    {
        return Task.CompletedTask;
    }

    public override Task<RenewEphemeralResource.Types.Response> RenewEphemeralResource(RenewEphemeralResource.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.RenewEphemeralResource.Types.Response());
    }

    public override Task<UnlockState.Types.Response> UnlockState(UnlockState.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.UnlockState.Types.Response());
    }

    public override Task<UpgradeResourceIdentity.Types.Response> UpgradeResourceIdentity(UpgradeResourceIdentity.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.UpgradeResourceIdentity.Types.Response());
    }

    public override Task<ValidateActionConfig.Types.Response> ValidateActionConfig(ValidateActionConfig.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.ValidateActionConfig.Types.Response());
    }

    public override Task<ValidateEphemeralResourceConfig.Types.Response> ValidateEphemeralResourceConfig(ValidateEphemeralResourceConfig.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.ValidateEphemeralResourceConfig.Types.Response());
    }

    public override Task<ValidateListResourceConfig.Types.Response> ValidateListResourceConfig(ValidateListResourceConfig.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.ValidateListResourceConfig.Types.Response());
    }

    public override Task<ValidateStateStore.Types.Response> ValidateStateStoreConfig(ValidateStateStore.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new ValidateStateStore.Types.Response());
    }

    public override Task<WriteStateBytes.Types.Response> WriteStateBytes(IAsyncStreamReader<WriteStateBytes.Types.RequestChunk> requestStream, ServerCallContext context)
    {
        return Task.FromResult(new Tfplugin6.WriteStateBytes.Types.Response());
    }
}
