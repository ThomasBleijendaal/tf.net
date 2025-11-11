using System.ComponentModel;
using MessagePack;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class SampleFileResource
{
    [Key("id")]
    [Computed]
    [Description("Unique ID for this resource.")]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
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
