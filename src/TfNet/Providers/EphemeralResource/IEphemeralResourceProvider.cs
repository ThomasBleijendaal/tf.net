using TfNet.Models;

namespace TfNet.Providers.EphemeralResource;

public interface IEphemeralResourceProvider<T>
{
    Task<EphemeralResult<T>> OpenAsync(T config);

    Task<EphemeralResult> RenewAsync();

    Task CloseAsync();
}
