using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Tfplugin6;

namespace TfNet.Serialization;

public class ComputedStringsValueFormatter : IMessagePackFormatter<string?[]?>
{
    public string?[]? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        else if (reader.NextMessagePackType == MessagePackType.Extension && reader.TryReadExtensionFormatHeader(out var extHeader) && extHeader.TypeCode == 0)
        {
            reader.Skip();
            return null;
        }

        if (reader.TryReadArrayHeader(out var count) && count > 0)
        {
            var output = new string?[count];
            for (var i = 0; i < count; i++)
            {
                output[i] = reader.ReadString();
            }
            return output;
        }

        return [];
    }

    public void Serialize(ref MessagePackWriter writer, string?[]? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteExtensionFormat(new ExtensionResult(0, new byte[1]));
            return;
        }

        writer.WriteArrayHeader(value.Length);
        foreach (var item in value)
        {
            if (item is not null)
            {
                writer.Write(Encoding.UTF8.GetBytes(item));
            }
            else
            {
                writer.WriteNil();
            }
        }
    }
}

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

        public DynamicValue SerializeDynamicValue<T>(T value)
        {
            return new DynamicValue
            {
                Msgpack = Google.Protobuf.ByteString.CopyFrom(serializer.SerializeMsgPack(value))
            };
        }
    }
}
