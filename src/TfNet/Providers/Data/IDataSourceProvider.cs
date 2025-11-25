namespace TfNet.Providers.Data;

public interface IDataSourceProvider<T>
{
    Task<T> ReadAsync(T request);
}
