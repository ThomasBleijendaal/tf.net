using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.Providers.Data;
using TfNet.Providers.ProviderConfig;
using TfNet.Providers.Resource;
using TfNet.Providers.Validation;
using TfNet.Schemas;
using Tfplugin6;

namespace TfNet.Registry;

internal class ResourceRegistry : IAsyncInitialized
{
    private readonly IEnumerable<ISchemaProvider> _schemaProviders;
    private readonly Dictionary<string, ValidatorRegistryRegistration> _validatorRegistrations;
    private readonly Dictionary<string, ResourceRegistryRegistration> _resourceRegistrations;
    private readonly Dictionary<string, DataSourceRegistryRegistration> _dataSourceRegistrations;

    public ResourceRegistry(
        IEnumerable<ISchemaProvider> schemaProviders,
        IEnumerable<ValidatorRegistryRegistration> validatorRegistrations,
        IEnumerable<ResourceRegistryRegistration> resourceRegistrations,
        IEnumerable<DataSourceRegistryRegistration> dataSourceRegistrations)
    {
        _schemaProviders = schemaProviders;
        _validatorRegistrations = validatorRegistrations.ToDictionary(x => x.ResourceName);
        _resourceRegistrations = resourceRegistrations.ToDictionary(x => x.ResourceName);
        _dataSourceRegistrations = dataSourceRegistrations.ToDictionary(x => x.ResourceName);
    }

    public async Task InitializeAsync()
    {
    }

    public IAsyncEnumerable<Registration<Schema>> GetSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.Resource);

    public IAsyncEnumerable<Registration<Schema>> GetDataSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.DataResource);

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


    public Dictionary<string, Type> DataTypes { get; } = new Dictionary<string, Type>();

    private async IAsyncEnumerable<Registration<Schema>> GetSchemasOfTypeAsync(SchemaType schemaType)
    {
        foreach (var schemaProvider in _schemaProviders.Where(x => x.Type == schemaType))
        {
            var schema = await schemaProvider.GetSchemaAsync();

            yield return new(schemaProvider.SchemaName, schema);
        }
    }

    private static T? Construct<T>(IServiceProvider sp, Type type) where T : class
        => sp.GetService(type) as T;
}
