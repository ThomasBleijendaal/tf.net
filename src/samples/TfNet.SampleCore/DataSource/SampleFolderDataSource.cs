using Nerdbank.MessagePack;
using PolyType;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.DataSource;

[SchemaVersion(1)]
//[GenerateShape]
public class SampleFolderDataSource
{
    [PropertyShape(Name = "path")]
    [Description("Path")]
    [Required]
    [MessagePackConverter(typeof(ComputedValueFormatter<string?>))]
    public string? Path { get; set; } = null!;

    [PropertyShape(Name = "files")]
    [Description("Files in folder")]
    [Computed]
    [MessagePackConverter(typeof(ComputedValueFormatter<string?[]?>))]
    public string?[]? Files { get; set; }
}
