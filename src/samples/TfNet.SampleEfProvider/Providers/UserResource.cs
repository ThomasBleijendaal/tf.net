using MessagePack;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleEfProvider.Providers;

[SchemaVersion(1)]
[MessagePackObject]
public class UserResource
{
    [Key("id")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedValueFormatter<string?>))]
    public string? Id { get; set; }

    [Key("name")]
    public string? Name { get; set; }

    [Key("email")]
    public string? Email { get; set; }

    [Key("roles")]
    public int[]? Roles { get; set; }
}
