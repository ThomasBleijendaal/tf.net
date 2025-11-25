using System.Reflection;
using MessagePack;
using TfNet.Extensions;

namespace TfNet.Models;

public record AttributePath(params string[] Path)
{
    public static AttributePath For<T>(params Span<string> names) => For(typeof(T), names);

    private static AttributePath For(Type type, params Span<string> names)
    {
        if (names.Length == 0)
        {
            throw new ArgumentException("Empty list given", nameof(names));
        }

        var propertyType = type.GetProperty(names[0])
            ?? throw new ArgumentException($"Property {names[0]} not found", nameof(names));

        var attribute = propertyType.GetCustomAttribute<KeyAttribute>();
        var name = attribute?.StringKey ?? names[0].ToFirstLetterLower();

        if (names.Length == 1)
        {
            return new AttributePath(name);
        }
        else
        {
            var result = For(propertyType.PropertyType, names[..1]);

            return result with
            {
                Path = [name, .. result.Path]
            };
        }
    }
}
