using Tfplugin6;

namespace TfNet.Providers.EphemeralResource;

internal interface IEphemeralResourceProviderHost
{
    Task<OpenEphemeralResource.Types.Response> OpenAsync(OpenEphemeralResource.Types.Request request);
    Task<RenewEphemeralResource.Types.Response> RenewAsync(RenewEphemeralResource.Types.Request request);
    Task<CloseEphemeralResource.Types.Response> CloseAsync(CloseEphemeralResource.Types.Request request);
}
