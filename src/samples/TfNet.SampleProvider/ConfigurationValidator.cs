using FluentValidation;

namespace TfNet.SampleProvider;

internal class ConfigurationValidator : FluentBaseValidator<Configuration>
{
    public ConfigurationValidator()
    {
        RuleFor(x => x.FileHeader).MaximumLength(3);
    }
}

