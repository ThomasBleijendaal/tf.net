using Nerdbank.MessagePack;
using PolyType;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.Resource;

[SchemaVersion(1)]
[Description("Sample file")]
public class SampleFileResource
{
    [PropertyShape(Name = "id")]
    [Computed]
    [Description("Unique ID for this resource.")]
    [MessagePackConverter(typeof(ComputedValueFormatter<string?>))]
    public string? Id { get; set; }

    [PropertyShape(Name = "path")]
    [Description("Path to the file.")]
    [Required]
    public string Path { get; set; } = null!;

    [PropertyShape(Name = "content")]
    [Description("Contents of the file.")]
    [Required]
    public string Content { get; set; } = null!;

    [PropertyShape(Name = "property")]
    [NestedBlock(MaxItems = 10, MinItems = 0)]
    public List<SampleFileProperty>? Properties { get; set; }

    [Description("File property")]
    public class SampleFileProperty
    {
        [PropertyShape(Name = "key")]
        [Description("Key of the property.")]
        [Required]
        public string Key { get; set; } = null!;

        [PropertyShape(Name = "value")]
        [Description("Value of the property.")]
        [Required]
        public string Value { get; set; } = null!;
    }
}
