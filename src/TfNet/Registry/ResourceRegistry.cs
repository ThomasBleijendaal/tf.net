using TfNet.Providers.Data;
using TfNet.Providers.Resource;
using TfNet.Providers.Validation;
using TfNet.Schemas;
using Tfplugin6;

namespace TfNet.Registry;

internal class ResourceRegistry
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ISchemaProvider> _schemaProviders;
    private readonly Dictionary<string, ValidatorRegistryRegistration> _validatorRegistrations;
    private readonly Dictionary<string, ResourceRegistryRegistration> _resourceRegistrations;
    private readonly Dictionary<string, DataSourceRegistryRegistration> _dataSourceRegistrations;

    public ResourceRegistry(
        IServiceProvider serviceProvider,
        IEnumerable<ISchemaProvider> schemaProviders,
        IEnumerable<ValidatorRegistryRegistration> validatorRegistrations,
        IEnumerable<ResourceRegistryRegistration> resourceRegistrations,
        IEnumerable<DataSourceRegistryRegistration> dataSourceRegistrations)
    {
        _serviceProvider = serviceProvider;
        _schemaProviders = schemaProviders;
        _validatorRegistrations = validatorRegistrations.ToDictionary(x => x.ResourceName);
        _resourceRegistrations = resourceRegistrations.ToDictionary(x => x.ResourceName);
        _dataSourceRegistrations = dataSourceRegistrations.ToDictionary(x => x.ResourceName);
    }

    public IAsyncEnumerable<Registration<Schema>> GetSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.Resource);

    public IAsyncEnumerable<Registration<Schema>> GetDataSchemasAsync() => GetSchemasOfTypeAsync(SchemaType.DataResource);

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

    public Dictionary<string, Schema> Schemas { get; } = new Dictionary<string, Schema>();

    public Dictionary<string, Schema> DataSchemas { get; } = new Dictionary<string, Schema>();

    public IResourceProviderHost? GetResourceProvider(string name)
        => _resourceRegistrations.TryGetValue(name, out var registration)
            ? Construct<IResourceProviderHost>(typeof(ResourceProviderHost<>).MakeGenericType(registration.Type))
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

    private T? Construct<T>(Type type) where T : class
        => _serviceProvider.GetService(type) as T;
}
