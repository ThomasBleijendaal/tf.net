using TfNet.Schemas;

namespace TfNet.SampleCore.DynamicFunction;

public class DynamicFunctionProvider : IFunctionHandler
{
    public ValueTask<Dictionary<string, FunctionSignature>> GetFunctionsAsync()
    {
        var result = new Dictionary<string, FunctionSignature>
        {
            ["policy1"] = new FunctionSignature
            {
                MarkdownDescription = """
                This is some description

                ```xml
                <wow>even code</wow>
                ```
                """,
                Summary = "This is some summary",
                Request = new Dictionary<string, FunctionSignature.TypeInfo>
                {
                    ["name"] = new(typeof(string)),
                    ["limit"] = new(typeof(int))
                },
                Response = typeof(string)
            },
            ["policy2"] = new FunctionSignature
            {
                Request = new Dictionary<string, FunctionSignature.TypeInfo>
                {
                    ["value"] = new(typeof(string))
                },
                Response = typeof(string)
            }
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
