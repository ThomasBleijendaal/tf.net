using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Schemas;

internal class ParameterSetter<T> : IParameterSetter
{
    private readonly Parameter[] _parameters;

    public ParameterSetter(Parameter[] parameters)
    {
        _parameters = parameters;
    }

    public void SetRequest(
        IDynamicValueSerializer serializer,
        CallFunction.Types.Request request,
        T target)
    {
        var i = 0;

        foreach (var arg in request.Arguments)
        {
            if (i >= _parameters.Length)
            {
                throw new ArgumentException("Invalid request");
            }

            var (type, setter) = _parameters[i];

            var value = serializer.DeserializeDynamicValue(type, arg);

            setter.Invoke(target, value);

            i++;
        }
    }

    public void SetRequest(
        IDynamicValueSerializer serializer,
        CallFunction.Types.Request request,
        object target)
            => SetRequest(serializer, request, (T)target);

    public record struct Parameter(Type Type, Action<T, object?> Setter);
}
