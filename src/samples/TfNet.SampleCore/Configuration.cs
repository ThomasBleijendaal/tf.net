using System.ComponentModel;
using PolyType;

namespace TfNet.SampleCore;

public class Configuration
{
    [PropertyShape(Name = "file_header")]
    [Description("Header text to prepend to every file.")]
    public string? FileHeader { get; set; }
}
