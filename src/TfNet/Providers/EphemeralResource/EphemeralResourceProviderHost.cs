using Google.Protobuf.WellKnownTypes;
using TfNet;
using TfNet.Extensions;
using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.EphemeralResource;

internal class EphemeralResourceProviderHost<T> : IEphemeralResourceProviderHost
{
    private readonly IEphemeralResourceProvider<T> _ephemeralResourceProvider;
    private readonly IDynamicValueSerializer _serializer;

    public EphemeralResourceProviderHost(
        IEphemeralResourceProvider<T> ephemeralResourceProvider,
        IDynamicValueSerializer serializer)
    {
        _ephemeralResourceProvider = ephemeralResourceProvider;
        _serializer = serializer;
    }

    public async Task<OpenEphemeralResource.Types.Response> OpenAsync(OpenEphemeralResource.Types.Request request)
    {
        var config = _serializer.DeserializeDynamicValue<T>(request.Config);
        if (config == null)
        {
            return new OpenEphemeralResource.Types.Response
            {
                Diagnostics =
                {
                    new Diagnostic
                    {
                        Summary = "Failed to deserialize config",
                        Severity = Diagnostic.Types.Severity.Invalid
                    }
                }
            };
        }

        var result = await _ephemeralResourceProvider.OpenAsync(config);
        var resultSerialized = _serializer.SerializeDynamicValue(result.Value);

        return new OpenEphemeralResource.Types.Response
        {
            Result = resultSerialized,
            RenewAt = Timestamp.FromDateTimeOffset(result.RenewAt)
        };
    }

    public async Task<RenewEphemeralResource.Types.Response> RenewAsync(RenewEphemeralResource.Types.Request request)
    {
        var result = await _ephemeralResourceProvider.RenewAsync();

        return new RenewEphemeralResource.Types.Response
        {
            RenewAt = Timestamp.FromDateTimeOffset(result.RenewAt)
        };
    }

    public async Task<CloseEphemeralResource.Types.Response> CloseAsync(CloseEphemeralResource.Types.Request request)
    {
        await _ephemeralResourceProvider.CloseAsync();

        return new CloseEphemeralResource.Types.Response();
    }
}
