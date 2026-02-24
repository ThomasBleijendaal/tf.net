using TfNet.Schemas;

namespace TfNet.Registry;

public interface IResourceRegistryContext
{
    IResourceRegisterer<T> RegisterResource<T>(string resourceName);

    IDataSourceRegisterer<T> RegisterDataSource<T>(string dataSourceName);

    IResourceRegisterer<T> RegisterEphemeralResource<T>(string ephemeralResourceName);

    IFunctionRegisterer<TRequest> RegisterFunction<TRequest, TResponse>(string functionName)
        where TRequest : new();

    void RegisterFunctions<TFunctionHandler>(string functionNamePrefix)
        where TFunctionHandler : IFunctionHandler;
}
