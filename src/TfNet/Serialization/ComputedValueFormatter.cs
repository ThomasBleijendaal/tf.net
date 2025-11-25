using MessagePack;
using MessagePack.Formatters;

namespace TfNet.Serialization;

public sealed class ComputedValueFormatter<T> : IMessagePackFormatter<T>
{
    public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default!;
        }
        else if (reader.NextMessagePackType == MessagePackType.Extension && reader.TryReadExtensionFormatHeader(out var extHeader) && extHeader.TypeCode == 0)
        {
            reader.Skip();
            return default!;
        }

        var formatter = options.Resolver.GetFormatter<T>()
            ?? throw new InvalidOperationException($"Cannot find message pack formatter for {typeof(T).Name}");

        return formatter.Deserialize(ref reader, options);
    }

    public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            writer.WriteExtensionFormat(new ExtensionResult(0, new byte[1]));
            return;
        }

        var formatter = options.Resolver.GetFormatter<T>()
            ?? throw new InvalidOperationException($"Cannot find message pack formatter for {typeof(T).Name}");

        formatter.Serialize(ref writer, value, options);
    }
}
