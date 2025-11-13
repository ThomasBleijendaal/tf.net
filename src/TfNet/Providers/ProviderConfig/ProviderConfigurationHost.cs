using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.ProviderConfig;

internal class ProviderConfigurationHost<T> : IProviderConfigurationHost
{
    private readonly IProviderConfigurator<T> _providerConfigurator;
    private readonly IDynamicValueSerializer _serializer;

    public ProviderConfigurationHost(
        IProviderConfigurator<T> providerConfigurator,
        IDynamicValueSerializer serializer)
    {
        _providerConfigurator = providerConfigurator;
        _serializer = serializer;
    }

    public Task ConfigureAsync(ConfigureProvider.Types.Request request)
    {
        var config = _serializer.DeserializeDynamicValue<T>(request.Config);
        return _providerConfigurator.ConfigureAsync(config);
    }
}
