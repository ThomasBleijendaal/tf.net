namespace TfNet.Providers.ProviderConfig;

public interface IProviderConfigurator<T>
{
    Task ConfigureAsync(T config);
}
