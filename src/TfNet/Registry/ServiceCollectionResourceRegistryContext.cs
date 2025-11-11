using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.Providers.Data;
using TfNet.Providers.Resource;
using TfNet.Providers.Validation;
using TfNet.Schemas;

namespace TfNet.Registry;

internal class ServiceCollectionResourceRegistryContext : IResourceRegistryContext
{
    private readonly IServiceCollection _services;

    public ServiceCollectionResourceRegistryContext(IServiceCollection services)
    {
        _services = services;
    }

    public void RegisterResource<T>(string resourceName)
    {
        EnsureValidType<T>();

        _services.AddSingleton<ISchemaProvider>(
            sp => sp.BuildService<TypeSchemaProvider<T>>([resourceName, SchemaType.Resource]));

        _services.AddSingleton(new ResourceRegistryRegistration(resourceName, typeof(T)));

        // TODO: move this to resource builder setup returned by this method
        // TODO: add support for the Required attribute in the validator
        _services.AddSingleton<IValidationProvider<T>, DataAnnotationValidationProvider<T>>();
        _services.AddSingleton<ValidationProviderHost<T>>();
        _services.AddSingleton(new ValidatorRegistryRegistration(resourceName, typeof(T)));
    }

    public void RegisterDataSource<T>(string resourceName)
    {
        EnsureValidType<T>();

        _services.AddSingleton<ISchemaProvider>(
            sp => sp.BuildService<TypeSchemaProvider<T>>([resourceName, SchemaType.DataResource]));

        _services.AddSingleton(new DataSourceRegistryRegistration(resourceName, typeof(T)));
    }

    private static void EnsureValidType<T>()
    {
        // Validation
        if (!typeof(T).IsPublic)
        {
            // Must be public to allow messagepack serialization.
            throw new InvalidOperationException($"Type {typeof(T).FullName} must be public in order to be used as a Terraform resource.");
        }
    }
}
