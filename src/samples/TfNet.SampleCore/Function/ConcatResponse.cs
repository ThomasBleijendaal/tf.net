using PolyType;

namespace TfNet.SampleCore.Function;

public class ConcatResponse
{
    [PropertyShape(Name = "result")]
    public string? Result { get; set; }
}
