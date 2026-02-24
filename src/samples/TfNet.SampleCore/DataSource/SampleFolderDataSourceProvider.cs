using TfNet.Providers.Data;

namespace TfNet.SampleCore.DataSource;

public class SampleFolderDataSourceProvider : IDataSourceProvider<SampleFolderDataSource>
{
    public Task<SampleFolderDataSource> ReadAsync(SampleFolderDataSource request)
    {
        var files = Directory.GetFiles(request.Path ?? "./");

        return Task.FromResult(request with
        {
            Path = request.Path,
            Files = files
        });
    }
}
