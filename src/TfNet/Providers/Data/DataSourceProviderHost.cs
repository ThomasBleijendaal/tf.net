using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Data;

internal class DataSourceProviderHost<T> : Host<T>
{
    private readonly IDataSourceProvider<T> _dataSourceProvider;

    public DataSourceProviderHost(
        IDataSourceProvider<T> dataSourceProvider,
        IDynamicValueSerializer serializer) : base(serializer)
    {
        _dataSourceProvider = dataSourceProvider;
    }

    public async Task<ReadDataSource.Types.Response> ReadDataSourceAsync(ReadDataSource.Types.Request request)
    {
        var current = DeserializeDynamicValue(request.Config);

        var read = await _dataSourceProvider.ReadAsync(current);
        var readSerialized = SerializeDynamicValue(read);

        return new ReadDataSource.Types.Response
        {
            State = readSerialized,
        };
    }
}
