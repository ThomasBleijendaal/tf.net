namespace TfNet.Providers.Function;

public interface IFunctionProvider<TRequest, TResponse>
{
    Task<TResponse> CallAsync(TRequest request);
}
