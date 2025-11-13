using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Schemas;

public interface IParameterSetter
{
    void SetRequest(
        IDynamicValueSerializer serializer,
        CallFunction.Types.Request request,
        object target);
}
