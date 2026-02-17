using Tfplugin6;

namespace TfNet.Serialization;

internal static class DynamicValueSerializerExtensions
{
    extension(IDynamicValueSerializer serializer)
    {
        public T? DeserializeDynamicValue<T>(DynamicValue value)
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

        public DynamicValue SerializeDynamicValue(Type type, object value)
        {
            var data = serializer.SerializeMsgPack(type, value);
            return new DynamicValue
            {
                Msgpack = Google.Protobuf.ByteString.CopyFrom(data)
            };
        }

        public DynamicValue SerializeDynamicValue<T>(T value)
        {
            if (typeof(T) == typeof(object))
            {
                throw new ArgumentException("Value cannot be of type object", nameof(value));
            }

            var data = serializer.SerializeMsgPack(value);
            return new DynamicValue
            {
                Msgpack = Google.Protobuf.ByteString.CopyFrom(data)
            };
        }
    }
}
