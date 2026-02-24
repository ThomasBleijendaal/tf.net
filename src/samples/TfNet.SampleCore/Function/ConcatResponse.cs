using PolyType;
using TfNet.Resources;

namespace TfNet.SampleCore.Function;

[SchemaVersion(1)]
public record ConcatResponse
{
    [PropertyShape(Name = "result")]
    public string? Result { get; set; }
}
