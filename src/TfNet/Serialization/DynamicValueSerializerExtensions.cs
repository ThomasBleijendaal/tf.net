using Tfplugin6;

namespace TfNet.Serialization;

internal static class DynamicValueSerializerExtensions
{
    extension(IDynamicValueSerializer serializer)
    {
        public T DeserializeDynamicValue<T>(DynamicValue value)
        {
            if (!value.Msgpack.IsEmpty)
            {
                return serializer.DeserializeMsgPack<T>(value.Msgpack.Memory);
            }

            if (!value.Json.IsEmpty)
            {
                return serializer.DeserializeJson<T>(value.Json.Memory);
            }

            throw new ArgumentException("Either MessagePack or Json must be non-empty.", nameof(value));
        }

        public object? DeserializeDynamicValue(Type type, DynamicValue value)
        {
            if (!value.Msgpack.IsEmpty)
            {
                return serializer.DeserializeMsgPack(type, value.Msgpack.Memory);
            }

            if (!value.Json.IsEmpty)
            {
                return serializer.DeserializeJson(type, value.Json.Memory);
            }

            throw new ArgumentException("Either MessagePack or Json must be non-empty.", nameof(value));
        }

        public DynamicValue SerializeDynamicValue<T>(T value)
        {
            return new DynamicValue
            {
                Msgpack = Google.Protobuf.ByteString.CopyFrom(serializer.SerializeMsgPack(value))
            };
        }
    }
}
