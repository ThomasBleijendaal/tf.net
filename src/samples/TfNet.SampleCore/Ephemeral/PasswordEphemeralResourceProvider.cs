using TfNet.Models;
using TfNet.Providers.EphemeralResource;

namespace TfNet.SampleCore.Ephemeral;

public class PasswordEphemeralResourceProvider : IEphemeralResourceProvider<PasswordEphemeralResource>
{
    public Task<EphemeralResult<PasswordEphemeralResource>> OpenAsync(PasswordEphemeralResource config)
    {
        var password = string.Join("", Enumerable.Repeat("a", config.Length));
        return Task.FromResult(new EphemeralResult<PasswordEphemeralResource>
        {
            Value = config with { Password = password },
            RenewAt = DateTimeOffset.UtcNow.AddHours(1)
        });
    }

    public Task<EphemeralResult> RenewAsync()
        => Task.FromResult(new EphemeralResult
        {
            RenewAt = DateTimeOffset.UtcNow.AddHours(1)
        });

    public Task CloseAsync() => Task.CompletedTask;
}
