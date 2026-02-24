using PolyType;
using TfNet.Resources;

namespace TfNet.SampleCore.Function;

[SchemaVersion(1)]
public record ConcatRequest
{
    [PropertyShape(Name = "parameter1")]
    public string? Parameter1 { get; set; }

    [PropertyShape(Name = "parameter2")]
    public string? Parameter2 { get; set; }
}
