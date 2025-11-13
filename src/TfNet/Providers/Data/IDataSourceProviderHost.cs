using Tfplugin6;

namespace TfNet.Providers.Data;

internal interface IDataSourceProviderHost
{
    Task<ReadDataSource.Types.Response> ReadDataSourceAsync(ReadDataSource.Types.Request request);
}