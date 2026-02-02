using System.Text.Json;
using Nerdbank.MessagePack;
using PolyType.ReflectionProvider;

namespace TfNet.Serialization;

public class DefaultDynamicValueSerializer : IDynamicValueSerializer
{
    private readonly MessagePackSerializer _serializer = new();

    public T DeserializeJson<T>(ReadOnlyMemory<byte> value)
    {
        return JsonSerializer.Deserialize<T>(value.Span, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
             ?? throw new InvalidOperationException("Invalid Json provided");
    }

    public object? DeserializeJson(Type type, ReadOnlyMemory<byte> value)
    {
        return JsonSerializer.Deserialize(value.Span, type, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
             ?? throw new InvalidOperationException("Invalid Json provided");
    }

    public T? DeserializeMsgPack<T>(ReadOnlyMemory<byte> value)
    {
        var shape = ReflectionTypeShapeProvider.Default.GetTypeShape<T>();
        var reader = new MessagePackReader(value);

        return _serializer.Deserialize(ref reader, shape, default);
    }

    public object? DeserializeMsgPack(Type type, ReadOnlyMemory<byte> value)
    {
        var shape = ReflectionTypeShapeProvider.Default.GetTypeShape(type);
        var reader = new MessagePackReader(value);

        return _serializer.DeserializeObject(ref reader, shape, default);
    }

    byte[] IDynamicValueSerializer.SerializeMsgPack<T>(T value)
    {
        var shape = ReflectionTypeShapeProvider.Default.GetTypeShape<T>();

        return _serializer.Serialize(value, shape);
    }
}
