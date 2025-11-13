using MessagePack;

namespace TfNet.SampleProvider.Function;

[MessagePackObject]
public class ConcatResponse
{
    [Key("result")]
    public string? Result { get; set; }
}
