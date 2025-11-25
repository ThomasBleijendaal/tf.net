using AsyncPlinq;
using Grpc.Core;
using Tfplugin6;

namespace TfNet.PluginCore.Services;

internal partial class Terraform6ProviderService : Provider.ProviderBase
{
    public override async Task<ConfigureProvider.Types.Response> ConfigureProvider(ConfigureProvider.Types.Request request, ServerCallContext context)
    {
        if (_resourceRegistry.GetProviderConfigurator(_serviceProvider) is { } configurator)
        {
            await configurator.ConfigureAsync(request);
        }

        return new ConfigureProvider.Types.Response { };
    }

    public override async Task<ValidateProviderConfig.Types.Response> ValidateProviderConfig(ValidateProviderConfig.Types.Request request, ServerCallContext context)
    {
        var response = new ValidateProviderConfig.Types.Response();

        // only validate when there is a validation provider registered
        if (_resourceRegistry.GetValidationProvider(_serviceProvider, Constants.Provider) is { } provider)
        {
            response.Diagnostics.AddRange(await provider.ValidateAsync(request.Config));
        }
        return response;
    }

    public override async Task<GetProviderSchema.Types.Response> GetProviderSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
    {
        var res = new GetProviderSchema.Types.Response
        {
            Provider = _providerConfiguration == null
                ? new Schema { Block = new Schema.Types.Block { } }
                : await _providerConfiguration.SchemaProvider.GetSchemaAsync(),
        };

        var resourceSchemas = await _resourceRegistry.GetSchemasAsync().ToArrayAsync();
        foreach (var (key, schema) in resourceSchemas)
        {
            res.ResourceSchemas.Add(key, schema);
        }

        var dataSchemas = await _resourceRegistry.GetDataSchemasAsync().ToArrayAsync();
        foreach (var (key, schema) in dataSchemas)
        {
            res.DataSourceSchemas.Add(key, schema);
        }

        var functions = await _resourceRegistry.GetFunctionsAsync().ToArrayAsync();
        foreach (var (key, function) in functions)
        {
            res.Functions.Add(key, function);
        }

        return res;
    }

    public override Task<StopProvider.Types.Response> StopProvider(StopProvider.Types.Request request, ServerCallContext context)
    {
        _lifetime.StopApplication();
        _lifetime.ApplicationStopped.WaitHandle.WaitOne();
        return Task.FromResult(new StopProvider.Types.Response());
    }
}
