namespace TfNet.Registry;

public interface IResourceRegistryContext
{
    IResourceRegisterer<T> RegisterResource<T>(string resourceName);

    IDataSourceRegisterer<T> RegisterDataSource<T>(string dataSourceName);

    IFunctionRegisterer<TRequest> RegisterFunction<TRequest, TResponse>(string functionName);
}
