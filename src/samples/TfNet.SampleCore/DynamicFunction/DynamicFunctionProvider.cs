using TfNet.Schemas;

namespace TfNet.SampleCore.DynamicFunction;

public class DynamicFunctionProvider : IFunctionHandler
{
    // ensures DI works
    private readonly SampleConfigurator _sampleConfigurator;

    public DynamicFunctionProvider(
        SampleConfigurator sampleConfigurator)
    {
        _sampleConfigurator = sampleConfigurator;
    }

    public ValueTask<Dictionary<string, FunctionSignature>> GetFunctionsAsync()
    {
        var result = new Dictionary<string, FunctionSignature>
        {
            ["policy1"] = new FunctionSignature(
                new Dictionary<string, Type>
                {
                    ["name"] = typeof(string),
                    ["limit"] = typeof(int)
                },
                typeof(string)),
            ["policy2"] = new FunctionSignature(
                new Dictionary<string, Type>
                {
                    ["value"] = typeof(string)
                },
                typeof(string))
        };

        return ValueTask.FromResult(result);
    }

    public ValueTask<object> HandleFunctionCallAsync(string functionName, Dictionary<string, object> requestParameters)
    {
        if (functionName == "policy1")
        {
            var name = requestParameters["name"];
            var limit = requestParameters["limit"];

            return ValueTask.FromResult<object>($"{name} has {limit}");
        }
        else if (functionName == "policy2")
        {
            var value = requestParameters["value"];

            return ValueTask.FromResult<object>($"Value is {value}");
        }
        else
        {
            throw new InvalidOperationException("Function not supported");
        }
    }
}
