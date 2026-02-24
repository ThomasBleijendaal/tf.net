using PolyType;

namespace TfNet.SampleCore.Function;

public class ConcatRequest
{
    [PropertyShape(Name = "parameter1")]
    public string? Parameter1 { get; set; }

    [PropertyShape(Name = "parameter2")]
    public string? Parameter2 { get; set; }
}
