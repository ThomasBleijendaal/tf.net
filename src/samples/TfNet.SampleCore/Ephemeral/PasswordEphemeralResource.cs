using FluentValidation;
using Nerdbank.MessagePack;
using PolyType;
using TfNet.Models;
using TfNet.Providers.EphemeralResource;
using TfNet.Resources;
using TfNet.Serialization;

namespace TfNet.SampleCore.Ephemeral;

[SchemaVersion(1)]
[Description("Sample password")]
// TODO: check if record can be used everywhere
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

public class PasswordEphemeralResourceProvider : IEphemeralResourceProvider<PasswordEphemeralResource>
{
    public Task<EphemeralResult<PasswordEphemeralResource>> OpenAsync(PasswordEphemeralResource config)
    {
        var password = string.Join("", Enumerable.Repeat("a", config.Length));
        return Task.FromResult(new EphemeralResult<PasswordEphemeralResource>
        {
            Value = config with { Password = password },
            RenewAt = DateTimeOffset.UtcNow.AddHours(1)
        });
    }

    public Task<EphemeralResult> RenewAsync()
        => Task.FromResult(new EphemeralResult
        {
            RenewAt = DateTimeOffset.UtcNow.AddHours(1)
        });

    public Task CloseAsync() => Task.CompletedTask;
}

public class PasswordEphemeralResourceValidator : FluentBaseValidator<PasswordEphemeralResource>
{
    public PasswordEphemeralResourceValidator()
    {
        RuleFor(x => x.Length).GreaterThan(7);
    }
}
