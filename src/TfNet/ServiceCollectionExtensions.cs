using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.Providers.Data;
using TfNet.Providers.Function;
using TfNet.Providers.ProviderConfig;
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
        services.AddTransient(typeof(FunctionProviderHost<,>));
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
    public static IProviderConfigRegisterer<TConfig> AddTerraformProviderConfigurator<TConfig, TProviderConfigurator>(this IServiceCollection services)
        where TProviderConfigurator : IProviderConfigurator<TConfig>
    {
        services.AddSingleton(sp => new ProviderConfigurationRegistry(
            SchemaProvider: sp.BuildService<TypeSchemaProvider<TConfig>>([Constants.Provider, SchemaType.Provider]),
            ConfigurationType: typeof(TConfig)));

        services.AddTransient<IProviderConfigurator<TConfig>>(s => s.GetRequiredService<TProviderConfigurator>());

        return new ServiceCollectionProviderConfigRegisterer<TConfig>(services, Constants.Provider);
    }
}
