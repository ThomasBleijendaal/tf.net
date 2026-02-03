using MessagePack;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleEfProvider.Providers;

[SchemaVersion(1)]
[MessagePackObject(SuppressSourceGeneration = true)]
public class UserRoleResource
{
    [Key("id")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedValueFormatter<int?>))]
    public int? Id { get; set; }

    [Key("name")]
    public string? RoleName { get; set; }
}
