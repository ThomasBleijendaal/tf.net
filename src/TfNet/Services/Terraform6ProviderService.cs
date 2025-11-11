using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TfNet.ProviderConfig;
using TfNet.Providers.Data;
using TfNet.Providers.Resource;
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

    public override async Task<ConfigureProvider.Types.Response> ConfigureProvider(ConfigureProvider.Types.Request request, ServerCallContext context)
    {
        if (_providerConfiguration == null)
        {
            return new ConfigureProvider.Types.Response { };
        }

        var configurationHostType = typeof(ProviderConfigurationHost<>).MakeGenericType(_providerConfiguration.ConfigurationType);
        var configurationHost = _serviceProvider.GetService(configurationHostType);
        await (Task)configurationHostType.GetMethod(nameof(ProviderConfigurationHost<>.ConfigureAsync))!
            .Invoke(configurationHost, new[] { request })!;
        return new ConfigureProvider.Types.Response { };
    }

    public override Task<ValidateProviderConfig.Types.Response> ValidateProviderConfig(ValidateProviderConfig.Types.Request request, ServerCallContext context)
    {
        var res = new ValidateProviderConfig.Types.Response();

        return Task.FromResult(res);
    }

    public override Task<GetResourceIdentitySchemas.Types.Response> GetResourceIdentitySchemas(GetResourceIdentitySchemas.Types.Request request, ServerCallContext context)
    {
        var res = new GetResourceIdentitySchemas.Types.Response();

        return Task.FromResult(res);
    }

    public override async Task<GetProviderSchema.Types.Response> GetProviderSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
    {
        var res = new GetProviderSchema.Types.Response
        {
            Provider = _providerConfiguration == null
                ? new Schema { Block = new Schema.Types.Block { } }
                : await _providerConfiguration.SchemaProvider.GetSchemaAsync(),
        };

        await foreach (var (key, schema) in _resourceRegistry.GetSchemasAsync())
        {
            res.ResourceSchemas.Add(key, schema);
        }

        await foreach (var (key, schema) in _resourceRegistry.GetDataSchemasAsync())
        {
            res.DataSourceSchemas.Add(key, schema);
        }

        return res;
    }

    public override async Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context)
        => _resourceRegistry.GetResourceProvider(request.TypeName) is { } provider
            ? await provider.PlanResourceChangeAsync(request)
            : new()
            {
                Diagnostics =
                {
                    new Diagnostic { Detail = "Unknown type name." },
                }
            };

    public override Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ApplyResourceChange.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ApplyResourceChange.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.ApplyResourceChangeAsync))!
            .Invoke(provider, new[] { request })!;
    }

    public override Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new UpgradeResourceState.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<UpgradeResourceState.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.UpgradeResourceStateAsync))!
            .Invoke(provider, new[] { request })!;
    }

    public override Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ReadResource.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ReadResource.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.ReadResourceAsync))!
            .Invoke(provider, new[] { request })!;
    }

    public override Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ImportResourceState.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ImportResourceState.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.ImportResourceStateAsync))!
            .Invoke(provider, new[] { request })!;
    }

    public override Task<StopProvider.Types.Response> StopProvider(StopProvider.Types.Request request, ServerCallContext context)
    {
        _lifetime.StopApplication();
        _lifetime.ApplicationStopped.WaitHandle.WaitOne();
        return Task.FromResult(new StopProvider.Types.Response());
    }

    public override Task<ValidateDataResourceConfig.Types.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Types.Request request, ServerCallContext context)
    {
        var res = new ValidateDataResourceConfig.Types.Response();

        return Task.FromResult(res);
    }

    public override Task<ValidateResourceConfig.Types.Response> ValidateResourceConfig(ValidateResourceConfig.Types.Request request, ServerCallContext context)
    {
        var res = new ValidateResourceConfig.Types.Response();

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
