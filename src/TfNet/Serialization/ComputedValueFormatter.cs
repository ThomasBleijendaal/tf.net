using Nerdbank.MessagePack;
using PolyType.ReflectionProvider;

namespace TfNet.Serialization;

public sealed class ComputedValueFormatter<T> : MessagePackConverter<T>
{
    public override T? Read(ref MessagePackReader reader, SerializationContext context)
    {
        if (reader.TryReadNil())
        {
            return default!;
        }
        else if (reader.NextMessagePackType == MessagePackType.Extension && reader.TryReadExtensionHeader(out var extHeader) && extHeader.TypeCode == 0)
        {
            reader.Skip(context);
            return default!;
        }

        var shape = ReflectionTypeShapeProvider.Default.GetTypeShape<T>();
        var converter = context.GetConverter(shape);

        return converter.Read(ref reader, context);
    }

    public override void Write(ref MessagePackWriter writer, in T? value, SerializationContext context)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            writer.Write(new Extension(0, new byte[1]));
            return;
        }

        var shape = ReflectionTypeShapeProvider.Default.GetTypeShape<T>();
        var converter = context.GetConverter(shape);

        converter.Write(ref writer, value, context);
    }
}
