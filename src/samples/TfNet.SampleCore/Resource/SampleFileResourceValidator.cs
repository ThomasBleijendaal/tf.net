using FluentValidation;

namespace TfNet.SampleCore.Resource;

public class SampleFileResourceValidator : FluentBaseValidator<SampleFileResource>
{
    public SampleFileResourceValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Path).NotEmpty();
    }
}
