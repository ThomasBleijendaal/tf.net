using MessagePack;

namespace TfNet.SampleProvider.Function;

[MessagePackObject]
public class ConcatRequest
{
    [Key("parameter1")]
    public string? Parameter1 { get; set; }

    [Key("parameter2")]
    public string? Parameter2 { get; set; }
}
