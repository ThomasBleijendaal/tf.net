using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.Providers.ProviderConfig;
using TfNet.Registry;
using TfNet.Schemas;

namespace TfNet.Plugin;

/// <summary>
/// Use the extensions in this class for more granular configuration of the Terraform plugin.
/// For simpler setup, use <see cref="TerraformPluginHost"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
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
