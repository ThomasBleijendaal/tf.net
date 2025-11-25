using TfNet.Providers.Function;

namespace TfNet.SampleCore.Function;

public class ConcatFunction : IFunctionProvider<ConcatRequest, ConcatResponse>
{
    public Task<ConcatResponse> CallAsync(ConcatRequest request)
    {
        return Task.FromResult(new ConcatResponse
        {
            Result = request.Parameter1 + request.Parameter2
        });
    }
}
