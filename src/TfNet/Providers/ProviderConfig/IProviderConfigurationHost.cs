using Tfplugin6;

namespace TfNet.Providers.ProviderConfig;

internal interface IProviderConfigurationHost
{
    Task ConfigureAsync(ConfigureProvider.Types.Request request);
}
