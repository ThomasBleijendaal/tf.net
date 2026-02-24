using Nerdbank.MessagePack;
using PolyType;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.Ephemeral;

[SchemaVersion(1)]
[Description("Sample password")]
public record PasswordEphemeralResource
{
    [PropertyShape(Name = "password")]
    [Computed]
    [Description("Generated password")]
    [MessagePackConverter(typeof(ComputedValueFormatter<string?>))]
    public string? Password { get; set; }

    [PropertyShape(Name = "length")]
    [Description("Length of the password")]
    [Required]
    public int Length { get; set; }
}
