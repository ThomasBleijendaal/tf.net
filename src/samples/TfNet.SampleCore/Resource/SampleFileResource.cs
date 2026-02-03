using MessagePack;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.Resource;

[SchemaVersion(1)]
[Description("Sample file")]
[MessagePackObject(SuppressSourceGeneration = true)] // for now disable source generation due to compile error
public class SampleFileResource
{
    [Key("id")]
    [Computed]
    [Description("Unique ID for this resource.")]
    [MessagePackFormatter(typeof(ComputedValueFormatter<string>))]
    public string? Id { get; set; }

    [Key("path")]
    [Description("Path to the file.")]
    [Required]
    public string Path { get; set; } = null!;

    [Key("content")]
    [Description("Contents of the file.")]
    [Required]
    public string Content { get; set; } = null!;

    [Key("property")]
    [NestedBlock(MaxItems = 10, MinItems = 1)]
    public List<SampleFileProperty>? Properties { get; set; }

    [Description("File property")]
    [MessagePackObject(SuppressSourceGeneration = true)] // for now disable source generation due to compile error
    public class SampleFileProperty
    {
        [Key("key")]
        [Description("Key of the property.")]
        [Required]
        public string Key { get; set; } = null!;

        [Key("value")]
        [Description("Value of the property.")]
        [Required]
        public string Value { get; set; } = null!;
    }
}
