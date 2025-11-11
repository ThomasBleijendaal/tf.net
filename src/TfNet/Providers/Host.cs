using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers;

internal abstract class Host<T>
{
    private readonly IDynamicValueSerializer _serializer;

    protected Host(
        IDynamicValueSerializer serializer)
    {
        _serializer = serializer;
    }


    protected T DeserializeDynamicValue(DynamicValue value)
    {
        if (!value.Msgpack.IsEmpty)
        {
            return _serializer.DeserializeMsgPack<T>(value.Msgpack.Memory);
        }

        if (!value.Json.IsEmpty)
        {
            return _serializer.DeserializeJson<T>(value.Json.Memory);
        }

        throw new ArgumentException("Either MessagePack or Json must be non-empty.", nameof(value));
    }

    protected DynamicValue SerializeDynamicValue(T value)
    {
        return new DynamicValue { Msgpack = Google.Protobuf.ByteString.CopyFrom(_serializer.SerializeMsgPack(value)) };
    }
}
