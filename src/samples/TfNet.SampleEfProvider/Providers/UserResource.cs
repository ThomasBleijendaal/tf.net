using Nerdbank.MessagePack;
using PolyType;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleEfProvider.Providers;

[SchemaVersion(1)]
public class UserResource
{
    [PropertyShape(Name = "id")]
    [Computed]
    [MessagePackConverter(typeof(ComputedValueFormatter<string?>))]
    public string? Id { get; set; }

    [PropertyShape(Name = "name")]
    public string? Name { get; set; }

    [PropertyShape(Name = "email")]
    public string? Email { get; set; }

    [PropertyShape(Name = "roles")]
    public int[]? Roles { get; set; }
}
