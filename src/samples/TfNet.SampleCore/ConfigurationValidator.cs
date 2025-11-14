using FluentValidation;

namespace TfNet.SampleCore;

public class ConfigurationValidator : FluentBaseValidator<Configuration>
{
    public ConfigurationValidator()
    {
        RuleFor(x => x.FileHeader).MaximumLength(3);
    }
}
