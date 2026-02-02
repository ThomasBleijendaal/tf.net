using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.Providers.Data;
using TfNet.Providers.Function;
using TfNet.Providers.Resource;
using TfNet.Schemas;

namespace TfNet.Registry;

internal class ServiceCollectionResourceRegistryContext : IResourceRegistryContext
{
    private readonly IServiceCollection _services;

    public ServiceCollectionResourceRegistryContext(IServiceCollection services)
    {
        _services = services;
    }

    public IResourceRegisterer<T> RegisterResource<T>(string resourceName)
    {
        EnsureValidType<T>();

        _services.AddSingleton<ISchemaProvider>(
            sp => sp.BuildService<TypeSchemaProvider<T>>([resourceName, SchemaType.Resource]));

        _services.AddSingleton(new ResourceRegistryRegistration(resourceName, typeof(T)));

        return new ServiceCollectionResourceRegisterer<T>(_services, resourceName);
    }

    public IDataSourceRegisterer<T> RegisterDataSource<T>(string dataSourceName)
    {
        EnsureValidType<T>();

        _services.AddSingleton<ISchemaProvider>(
            sp => sp.BuildService<TypeSchemaProvider<T>>([dataSourceName, SchemaType.DataResource]));

        _services.AddSingleton(new DataSourceRegistryRegistration(dataSourceName, typeof(T)));

        return new ServiceCollectionDataSourceRegisterer<T>(_services, dataSourceName);
    }

    public IFunctionRegisterer<TRequest> RegisterFunction<TRequest, TResponse>(string functionName)
        where TRequest : new()
    {
        EnsureValidType<TRequest>();
        EnsureValidType<TResponse>();

        _services.AddSingleton<IFunctionSchemaProvider>(
            sp => sp.BuildService<FunctionSchemaProvider<TRequest, TResponse>>([functionName]));

        _services.AddSingleton(new FunctionRegistryRegistration(functionName, typeof(TRequest), typeof(TResponse)));

        return new ServiceCollectionFunctionRegisterer<TRequest>(_services, functionName);
    }

    public void RegisterFunctions<TFunctionHandler>(string functionNamePrefix) where TFunctionHandler : IFunctionHandler
    {
        if (!functionNamePrefix.EndsWith('_'))
        {
            throw new ArgumentException("Value must end with underscore", nameof(functionNamePrefix));
        }

        _services.AddSingleton<IDynamicFunctionSchemaProvider>(
            sp => sp.BuildService<DynamicFunctionSchemaProvider<TFunctionHandler>>([functionNamePrefix]));
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
