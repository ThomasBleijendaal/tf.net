using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Schemas;

internal class FunctionSignatureSetter : IParameterSetter
{
    private readonly FunctionSignature _signature;

    public FunctionSignatureSetter(
        FunctionSignature signature)
    {
        _signature = signature;
    }

    public void SetRequest(IDynamicValueSerializer serializer, CallFunction.Types.Request request, object target)
    {
        if (target is not Dictionary<string, object?> dictionaryTarget)
        {
            throw new InvalidOperationException("Target must be a Dictionary<string, object?>");
        }

        var i = 0;

        var requestArguments = _signature.Request.ToArray();

        foreach (var arg in request.Arguments)
        {
            if (i >= requestArguments.Length)
            {
                throw new ArgumentException("Invalid request");
            }

            var (name, type) = requestArguments[i];

            var value = serializer.DeserializeDynamicValue(type, arg);

            dictionaryTarget[name] = value;

            i++;
        }
    }
}
