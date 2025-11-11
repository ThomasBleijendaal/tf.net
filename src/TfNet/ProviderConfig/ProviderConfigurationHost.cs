using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.ProviderConfig;

internal class ProviderConfigurationHost<T>
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
        var config = DeserializeDynamicValue(request.Config);
        return _providerConfigurator.ConfigureAsync(config);
    }

    private T DeserializeDynamicValue(DynamicValue value)
    {
        if (!value.Msgpack.IsEmpty)
        {
            return _serializer.DeserializeMsgPack<T>(value.Msgpack.Memory);
        }

        if (!value.Json.IsEmpty)
        {
            return _serializer.DeserializeJson<T>(value.Json.Memory);
        }

        throw new ArgumentException("Either MessagePack or Json must be non-empty.", nameof(value));
    }
}
