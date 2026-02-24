using FluentValidation;
using Nerdbank.MessagePack;
using TfNet.Serialization;

namespace TfNet.SampleCore.Ephemeral;

public class PasswordEphemeralResourceValidator : FluentBaseValidator<PasswordEphemeralResource>
{
    public PasswordEphemeralResourceValidator()
    {
        RuleFor(x => x.Length).GreaterThan(7);
    }
}
