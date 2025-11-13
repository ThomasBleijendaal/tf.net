namespace TfNet.Serialization;

public interface IDynamicValueSerializer
{
    object? DeserializeJson(Type type, ReadOnlyMemory<byte> value);

    T DeserializeJson<T>(ReadOnlyMemory<byte> value);

    object? DeserializeMsgPack(Type type, ReadOnlyMemory<byte> value);

    T DeserializeMsgPack<T>(ReadOnlyMemory<byte> value);

    byte[] SerializeMsgPack<T>(T value);
}
