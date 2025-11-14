using MessagePack;

namespace TfNet.SampleCore.Function;

[MessagePackObject]
public class ConcatRequest
{
    [Key("parameter1")]
    public string? Parameter1 { get; set; }

    [Key("parameter2")]
    public string? Parameter2 { get; set; }
}
