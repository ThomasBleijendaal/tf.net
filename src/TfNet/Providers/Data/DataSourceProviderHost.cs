using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Data;

internal class DataSourceProviderHost<T> : IDataSourceProviderHost
{
    private readonly IDataSourceProvider<T> _dataSourceProvider;
    private readonly IDynamicValueSerializer _serializer;

    public DataSourceProviderHost(
        IDataSourceProvider<T> dataSourceProvider,
        IDynamicValueSerializer serializer)
    {
        _dataSourceProvider = dataSourceProvider;
        _serializer = serializer;
    }

    public async Task<ReadDataSource.Types.Response> ReadDataSourceAsync(ReadDataSource.Types.Request request)
    {
        var current = _serializer.DeserializeDynamicValue<T>(request.Config);

        var read = await _dataSourceProvider.ReadAsync(current);
        var readSerialized = _serializer.SerializeDynamicValue(read);

        return new ReadDataSource.Types.Response
        {
            State = readSerialized,
        };
    }
}
