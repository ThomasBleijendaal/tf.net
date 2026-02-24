using Nerdbank.MessagePack;
using PolyType;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleEfProvider.Providers;

[SchemaVersion(1)]
public class UserRoleResource
{
    [PropertyShape(Name = "id")]
    [Computed]
    [MessagePackConverter(typeof(ComputedValueFormatter<int?>))]
    public int? Id { get; set; }

    [PropertyShape(Name = "name")]
    public string? RoleName { get; set; }
}
