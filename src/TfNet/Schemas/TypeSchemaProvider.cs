using System.Reflection;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using PolyType;
using TfNet.Resources;
using TfNet.Schemas.Types;
using Tfplugin6;

namespace TfNet.Schemas;

internal class TypeSchemaProvider<T> : ISchemaProvider
{
    private readonly ILogger<TypeSchemaProvider<T>> _logger;
    private readonly ITerraformTypeBuilder _typeBuilder;

    private Schema? _schema;

    public TypeSchemaProvider(
        string schemaName,
        SchemaType schemaType,
        ILogger<TypeSchemaProvider<T>> logger,
        ITerraformTypeBuilder typeBuilder)
    {
        _logger = logger;
        _typeBuilder = typeBuilder;

        SchemaName = schemaName;
        Type = schemaType;
    }

    public string SchemaName { get; }

    public SchemaType Type { get; }

    public ValueTask<Schema> GetSchemaAsync()
    {
        if (_schema != null)
        {
            return ValueTask.FromResult(_schema);
        }

        var type = typeof(T);

        var schemaVersionAttribute = type.GetCustomAttribute<SchemaVersionAttribute>();
        if (schemaVersionAttribute == null)
        {
            _logger.LogWarning($"Missing {nameof(SchemaVersionAttribute)} when generating schema for {type.FullName}.");
        }

        var properties = type.GetProperties();

        var block = ConvertPropertiesToBlock(type, properties);

        _schema = new Schema
        {
            Version = schemaVersionAttribute?.SchemaVersion ?? 0,
            Block = block,
        };

        return ValueTask.FromResult(_schema);
    }

    private Schema.Types.Block ConvertPropertiesToBlock(Type type, PropertyInfo[] properties)
    {
        var block = new Schema.Types.Block();

        var blockDescription = type.GetCustomAttribute<DescriptionAttribute>();
        block.Description = blockDescription?.MarkdownDescription ?? "";
        block.DescriptionKind = StringKind.Markdown;

        foreach (var property in properties)
        {
            var key = property.GetCustomAttribute<PropertyShapeAttribute>() ?? throw new InvalidOperationException($"Missing {nameof(PropertyShapeAttribute)} on {property.Name} in {type.Name}.");

            var description = property.GetCustomAttribute<DescriptionAttribute>();
            var required = TerraformTypeBuilder.IsRequiredAttribute(property);
            var isComputed = property.GetCustomAttribute<ComputedAttribute>() != null;
            var nestedBlock = property.GetCustomAttribute<NestedBlockAttribute>();

            if (nestedBlock == null)
            {
                var terraformType = _typeBuilder.GetTerraformType(property.PropertyType);
                if (terraformType is TerraformType.TfObject && !required)
                {
                    throw new InvalidOperationException("Optional object types are not supported.");
                }

                block.Attributes.Add(new Schema.Types.Attribute
                {
                    Name = key.Name,
                    Type = ByteString.CopyFromUtf8(terraformType.ToJson()),
                    Description = description?.MarkdownDescription ?? "",
                    DescriptionKind = StringKind.Markdown,
                    Optional = !required,
                    Required = required,
                    Computed = isComputed
                });
            }
            else
            {
                var propertyType = property.PropertyType;
                if (propertyType == null)
                {
                    continue;
                }

                var genericBaseType = !propertyType.IsGenericType ? null : propertyType.GetGenericTypeDefinition();

                var (nesting, nestedBlockType) = propertyType switch
                {
                    var single when genericBaseType == null => (Schema.Types.NestedBlock.Types.NestingMode.Single, single),
                    var array when propertyType.IsArray && propertyType.HasElementType => (Schema.Types.NestedBlock.Types.NestingMode.List, array.GetElementType()!),
                    var set when genericBaseType == typeof(List<>) => (Schema.Types.NestedBlock.Types.NestingMode.Set, set.GenericTypeArguments[0]),
                    var map when genericBaseType == typeof(Dictionary<,>) => (Schema.Types.NestedBlock.Types.NestingMode.Map, map.GenericTypeArguments[1]),
                    _ => throw new InvalidOperationException("Unsupported block type, only classes, arrays, lists and dictionaries are supported")
                };

                block.BlockTypes.Add(new Schema.Types.NestedBlock
                {
                    TypeName = key.Name,
                    MinItems = nestedBlock.MinItems,
                    MaxItems = nestedBlock.MaxItems,
                    Nesting = nesting,
                    Block = ConvertPropertiesToBlock(nestedBlockType, nestedBlockType.GetProperties())
                });
            }
        }

        return block;
    }
}
