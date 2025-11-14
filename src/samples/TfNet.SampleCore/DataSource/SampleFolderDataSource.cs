using System.ComponentModel;
using MessagePack;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.DataSource;

[SchemaVersion(1)]
[MessagePackObject]
public class SampleFolderDataSource
{
    [Key("path")]
    [Description("Path")]
    [Required]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string Path { get; set; } = null!;

    [Key("files")]
    [Description("Files in folder")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedStringsValueFormatter))]
    public string?[]? Files { get; set; }
}
