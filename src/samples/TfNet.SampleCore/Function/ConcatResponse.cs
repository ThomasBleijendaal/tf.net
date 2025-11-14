using MessagePack;

namespace TfNet.SampleCore.Function;

[MessagePackObject]
public class ConcatResponse
{
    [Key("result")]
    public string? Result { get; set; }
}
