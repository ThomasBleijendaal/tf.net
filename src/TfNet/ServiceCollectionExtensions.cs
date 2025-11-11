using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.ProviderConfig;
using TfNet.Providers.Data;
using TfNet.Providers.Resource;
using TfNet.Providers.ResourceUpgrade;
using TfNet.Registry;
using TfNet.Schemas;
using TfNet.Schemas.Types;
using TfNet.Serialization;

namespace TfNet;

/// <summary>
/// Use the extensions in this class for more granular configuration of the Terraform plugin.
/// For simpler setup, use <see cref="TerraformPluginHost"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTerraformPluginCore(this IServiceCollection services)
    {
        services.AddTransient<ITerraformTypeBuilder, TerraformTypeBuilder>();
        services.AddTransient(typeof(ProviderConfigurationHost<>));
        services.AddTransient(typeof(ResourceProviderHost<>));
        services.AddTransient(typeof(DataSourceProviderHost<>));
        services.AddTransient(typeof(IResourceUpgrader<>), typeof(DefaultResourceUpgrader<>));
        services.AddTransient<IDynamicValueSerializer, DefaultDynamicValueSerializer>();
        return services;
    }

    public static IResourceRegistryContext AddTerraformResourceRegistry(this IServiceCollection services)
    {
        services.AddOptions<TerraformPluginHostOptions>().ValidateDataAnnotations();
        services.AddSingleton<ResourceRegistry>();

        var registryContext = new ServiceCollectionResourceRegistryContext(services);
        return registryContext;
    }

    /// <summary>
    /// Adds a configurator that will be called when configuring this terraform plugin.
    /// </summary>
    public static IServiceCollection AddTerraformProviderConfigurator<TConfig, TProviderConfigurator>(this IServiceCollection services)
        where TProviderConfigurator : IProviderConfigurator<TConfig>
    {
        var schemaProviderType = typeof(TypeSchemaProvider<TConfig>);

        services.AddSingleton(sp => new ProviderConfigurationRegistry(
            SchemaProvider: sp.BuildService<TypeSchemaProvider<TConfig>>(["__provider", SchemaType.Provider]),
            ConfigurationType: typeof(TConfig)));

        services.AddTransient<IProviderConfigurator<TConfig>>(s => s.GetRequiredService<TProviderConfigurator>());
        return services;
    }
}
