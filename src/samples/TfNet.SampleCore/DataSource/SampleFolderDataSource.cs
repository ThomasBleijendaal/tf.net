using System.ComponentModel;
using MessagePack;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.DataSource;

[SchemaVersion(1)]
[MessagePackObject(SuppressSourceGeneration = true)] // for now disable source generation due to compile error
public class SampleFolderDataSource
{
    [Key("path")]
    [Description("Path")]
    [Required]
    [MessagePackFormatter(typeof(ComputedValueFormatter<string?>))]
    public string? Path { get; set; } = null!;

    [Key("files")]
    [Description("Files in folder")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedValueFormatter<string?[]?>))]
    public string?[]? Files { get; set; }
}
