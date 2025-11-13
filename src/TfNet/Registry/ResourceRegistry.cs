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
    private readonly IServiceProvider _serviceProvider;
    private readonly ProviderConfigurationRegistry? _providerConfigurationRegistry;
    private readonly IEnumerable<ISchemaProvider> _schemaProviders;
    private readonly IEnumerable<IFunctionProvider> _functionProviders;
    private readonly Dictionary<string, ValidatorRegistryRegistration> _validatorRegistrations;
    private readonly Dictionary<string, ResourceRegistryRegistration> _resourceRegistrations;
    private readonly Dictionary<string, DataSourceRegistryRegistration> _dataSourceRegistrations;
    private readonly Dictionary<string, FunctionRegistryRegistration> _functionRegistrations;

    public ResourceRegistry(
        IServiceProvider serviceProvider,
        ProviderConfigurationRegistry? providerConfigurationRegistry,
        IEnumerable<ISchemaProvider> schemaProviders,
        IEnumerable<IFunctionProvider> functionProviders,
        IEnumerable<ValidatorRegistryRegistration> validatorRegistrations,
        IEnumerable<ResourceRegistryRegistration> resourceRegistrations,
        IEnumerable<DataSourceRegistryRegistration> dataSourceRegistrations,
        IEnumerable<FunctionRegistryRegistration> functionRegistrations)
    {
        _serviceProvider = serviceProvider;
        _providerConfigurationRegistry = providerConfigurationRegistry;
        _schemaProviders = schemaProviders;
        _functionProviders = functionProviders;
        _validatorRegistrations = validatorRegistrations.ToDictionary(x => x.ResourceName);
        _resourceRegistrations = resourceRegistrations.ToDictionary(x => x.ResourceName);
        _dataSourceRegistrations = dataSourceRegistrations.ToDictionary(x => x.ResourceName);
        _functionRegistrations = functionRegistrations.ToDictionary(x => x.ResourceName);
    }

    public IAsyncEnumerable<Registration<Schema>> GetSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.Resource);

    public IAsyncEnumerable<Registration<Schema>> GetDataSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.DataResource);

    public IAsyncEnumerable<Registration<Function>> GetFunctionsAsync() => GetAllFunctionsAsync();

    public IValidationProviderHost? GetValidationProvider(string name)
    {
        if (_validatorRegistrations.TryGetValue(name, out var registration))
        {
            return Construct<IValidationProviderHost>(typeof(ValidationProviderHost<>).MakeGenericType(registration.Type));
        }
        else
        {
            return null;
        }
    }

    public IProviderConfigurationHost? GetProviderConfigurator()
    {
        if (_providerConfigurationRegistry is not null)
        {
            return Construct<IProviderConfigurationHost>(typeof(ProviderConfigurationHost<>).MakeGenericType(_providerConfigurationRegistry.ConfigurationType));
        }
        else
        {
            return null;
        }
    }

    public IResourceProviderHost? GetResourceProvider(string name)
        => _resourceRegistrations.TryGetValue(name, out var registration)
            ? Construct<IResourceProviderHost>(typeof(ResourceProviderHost<>).MakeGenericType(registration.Type))
            : null;

    public IDataSourceProviderHost? GetDataSourceProvider(string name)
        => _dataSourceRegistrations.TryGetValue(name, out var registration)
            ? Construct<IDataSourceProviderHost>(typeof(DataSourceProviderHost<>).MakeGenericType(registration.Type))
            : null;

    public IFunctionProviderHost? GetFunctionProvider(string name)
    {
        if (_functionRegistrations.TryGetValue(name, out var registration))
        {
            return Construct<IFunctionProviderHost>(typeof(FunctionProviderHost<,>).MakeGenericType(registration.Request, registration.Response));
        }
        else
        {
            return null;
        }
    }

    public Dictionary<string, Type> DataTypes { get; } = new Dictionary<string, Type>();

    private async IAsyncEnumerable<Registration<Schema>> GetSchemasOfTypeAsync(SchemaType schemaType)
    {
        foreach (var schemaProvider in _schemaProviders.Where(x => x.Type == schemaType))
        {
            var schema = await schemaProvider.GetSchemaAsync();

            yield return new(schemaProvider.SchemaName, schema);
        }
    }

    private async IAsyncEnumerable<Registration<Function>> GetAllFunctionsAsync()
    {
        foreach (var functionProvider in _functionProviders)
        {
            var function = await functionProvider.GetFunctionAsync();

            yield return new(functionProvider.FunctionName, function);
        }
    }

    private T? Construct<T>(Type type) where T : class
        => _serviceProvider.GetService(type) as T;
}
