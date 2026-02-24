using PolyType;
using TfNet.Resources;

namespace TfNet.SampleCore;

[SchemaVersion(1)]
public class Configuration
{
    [PropertyShape(Name = "file_header")]
    [Description("Header text to prepend to every file.")]
    public string? FileHeader { get; set; }
}
