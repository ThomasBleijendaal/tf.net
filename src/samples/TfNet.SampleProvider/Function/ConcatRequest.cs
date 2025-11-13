using MessagePack;
using TfNet.Resources;

namespace TfNet.SampleProvider.Function;

[SchemaVersion(1)]
[MessagePackObject]
public class ConcatRequest
{
    [Key("parameter1")]
    public string? Parameter1 { get; set; }

    [Key("parameter2")]
    public string? Parameter2 { get; set; }
}
