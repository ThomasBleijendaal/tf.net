using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.Providers.Data;
using TfNet.Providers.Function;
using TfNet.Providers.ProviderConfig;
using TfNet.Providers.Resource;
using TfNet.Providers.Validation;
using TfNet.Schemas;
using Tfplugin6;

namespace TfNet.Registry;

internal class ResourceRegistry
{
    private readonly IEnumerable<ISchemaProvider> _schemaProviders;
    private readonly IEnumerable<IDynamicFunctionSchemaProvider> _dynamicFunctionProviders;
    private readonly Dictionary<string, IFunctionSchemaProvider> _functionProviders;
    private readonly Dictionary<string, ValidatorRegistryRegistration> _validatorRegistrations;
    private readonly Dictionary<string, ResourceRegistryRegistration> _resourceRegistrations;
    private readonly Dictionary<string, DataSourceRegistryRegistration> _dataSourceRegistrations;
    private readonly Dictionary<string, IFunctionRegistration> _functionRegistrations;

    public ResourceRegistry(
        IEnumerable<ISchemaProvider> schemaProviders,
        IEnumerable<IFunctionSchemaProvider> functionProviders,
        IEnumerable<IDynamicFunctionSchemaProvider> dynamicFunctionProviders,
        IEnumerable<ValidatorRegistryRegistration> validatorRegistrations,
        IEnumerable<ResourceRegistryRegistration> resourceRegistrations,
        IEnumerable<DataSourceRegistryRegistration> dataSourceRegistrations,
        IEnumerable<FunctionRegistryRegistration> functionRegistrations)
    {
        _schemaProviders = schemaProviders;
        _dynamicFunctionProviders = dynamicFunctionProviders;
        _functionProviders = functionProviders.ToDictionary(x => x.FunctionName);
        _validatorRegistrations = validatorRegistrations.ToDictionary(x => x.ResourceName);
        _resourceRegistrations = resourceRegistrations.ToDictionary(x => x.ResourceName);
        _dataSourceRegistrations = dataSourceRegistrations.ToDictionary(x => x.ResourceName);
        _functionRegistrations = functionRegistrations.ToDictionary(x => x.ResourceName, x => (IFunctionRegistration)x);
    }

    public IAsyncEnumerable<Registration<Schema>> GetSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.Resource);

    public IAsyncEnumerable<Registration<Schema>> GetDataSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.DataResource);

    public IAsyncEnumerable<Registration<Function>> GetFunctionsAsync() => GetAllFunctionsAsync();

    public IValidationProviderHost? GetValidationProvider(IServiceProvider sp, string name)
    {
        if (_validatorRegistrations.TryGetValue(name, out var registration))
        {
            return Construct<IValidationProviderHost>(sp, typeof(ValidationProviderHost<>).MakeGenericType(registration.Type));
        }
        else
        {
            return null;
        }
    }

    public IProviderConfigurationHost? GetProviderConfigurator(IServiceProvider sp)
    {
        var registry = sp.GetService<ProviderConfigurationRegistry>();

        if (registry is not null)
        {
            return Construct<IProviderConfigurationHost>(sp, typeof(ProviderConfigurationHost<>).MakeGenericType(registry.ConfigurationType));
        }
        else
        {
            return null;
        }
    }

    public IResourceProviderHost? GetResourceProvider(IServiceProvider sp, string name)
        => _resourceRegistrations.TryGetValue(name, out var registration)
            ? Construct<IResourceProviderHost>(sp, typeof(ResourceProviderHost<>).MakeGenericType(registration.Type))
            : null;

    public IDataSourceProviderHost? GetDataSourceProvider(IServiceProvider sp, string name)
        => _dataSourceRegistrations.TryGetValue(name, out var registration)
            ? Construct<IDataSourceProviderHost>(sp, typeof(DataSourceProviderHost<>).MakeGenericType(registration.Type))
            : null;

    public async Task<IFunctionProviderHost?> GetFunctionProviderAsync(IServiceProvider sp, string name)
    {
        if (!_functionRegistrations.TryGetValue(name, out var registration))
        {
            // GetAllFunctionAsync() might not have run so run it again and try again
            await GetAllFunctionsAsync().ToListAsync();

            if (!_functionRegistrations.TryGetValue(name, out registration))
            {
                return null;
            }
        }

        if (registration is FunctionRegistryRegistration function)
        {
            return Construct<IFunctionProviderHost>(sp, typeof(FunctionProviderHost<,>).MakeGenericType(function.Request, function.Response));
        }
        else if (registration is DynamicFunctionRegistryRegistration dynamicFunction)
        {
            return sp.BuildService<DynamicFunctionProviderHost>([dynamicFunction]);
        }
        else
        {
            throw new NotSupportedException("FunctionRegistration not supported");
        }
    }

    public async ValueTask<IParameterSetter?> GetFunctionRequestSetterAsync(string name)
        => _functionProviders.TryGetValue(name, out var functionProvider)
            ? (await functionProvider.GetRequestSetterAsync())
            : null;

    public Dictionary<string, Type> DataTypes { get; } = new Dictionary<string, Type>();

    private async IAsyncEnumerable<Registration<Schema>> GetSchemasOfTypeAsync(SchemaType schemaType)
    {
        foreach (var schemaProvider in _schemaProviders.Where(x => x.Type == schemaType))
        {
            var schema = await schemaProvider.GetSchemaAsync();

            yield return new(schemaProvider.SchemaName, schema);
        }
    }

    // TODO: this needs to be refactored into an initialization step
    private async IAsyncEnumerable<Registration<Function>> GetAllFunctionsAsync()
    {
        foreach (var functionProvider in _functionProviders.Values)
        {
            var function = await functionProvider.GetFunctionSchemaAsync();

            yield return new(functionProvider.FunctionName, function);
        }

        foreach (var provider in _dynamicFunctionProviders)
        {
            var functions = await provider.GetFunctionSchemasAsync();

            foreach (var (name, functionProvider) in functions)
            {
                _functionProviders[name] = functionProvider;

                var handlerFunctionName = name.Replace(provider.FunctionNamePrefix, "");

                _functionRegistrations[name] = new DynamicFunctionRegistryRegistration(name, handlerFunctionName, provider.Handler);

                var function = await functionProvider.GetFunctionSchemaAsync();
                yield return new(name, function);
            }
        }
    }

    private static T? Construct<T>(IServiceProvider sp, Type type) where T : class
        => sp.GetService(type) as T;
}
