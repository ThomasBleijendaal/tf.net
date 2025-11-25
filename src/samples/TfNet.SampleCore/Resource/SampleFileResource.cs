using System.ComponentModel;
using MessagePack;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.Resource;

[SchemaVersion(1)]
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
}
