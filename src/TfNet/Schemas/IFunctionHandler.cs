namespace TfNet.Schemas;

public interface IFunctionHandler
{
    ValueTask<Dictionary<string, FunctionSignature>> GetFunctionsAsync();

    ValueTask<object> HandleFunctionCallAsync(string functionName, Dictionary<string, object> requestParameters);
}
