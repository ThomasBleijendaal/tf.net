using FluentValidation;

namespace TfNet.SampleProvider.Resource;

internal class SampleFileResourceValidator : FluentBaseValidator<SampleFileResource>
{
    public SampleFileResourceValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Path).NotEmpty();
    }
}
