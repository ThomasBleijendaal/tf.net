using TfNet.Providers.ProviderConfig;

namespace TfNet.SampleEfProvider;

public class SampleConfigurator : IProviderConfigurator<Configuration>
{
    public Configuration? Config { get; private set; }

    public Task ConfigureAsync(Configuration config)
    {
        Config = config;
        return Task.CompletedTask;
    }
}
