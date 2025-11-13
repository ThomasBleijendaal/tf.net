using Microsoft.Extensions.DependencyInjection;
using TfNet.Providers.Validation;

namespace TfNet.Registry;

internal class ServiceCollectionFunctionRegisterer<T> : IFunctionRegisterer<T>
{
    private readonly IServiceCollection _services;
    private readonly string _functionName;

    public ServiceCollectionFunctionRegisterer(
        IServiceCollection services,
        string functionName)
    {
        _services = services;
        _functionName = functionName;
    }

    public IFunctionRegisterer<T> WithDataAnnotationValidation()
    {
        _services.AddTransient<IValidationProvider<T>, DataAnnotationValidationProvider<T>>();
        return WithValidator<DataAnnotationValidationProvider<T>>();
    }

    public IFunctionRegisterer<T> WithValidator<TValidator>() where TValidator : class, IValidationProvider<T>
    {
        _services.AddTransient<ValidationProviderHost<T>>();
        _services.AddSingleton(new ValidatorRegistryRegistration(_functionName, typeof(T)));

        return this;
    }
}
