using Microsoft.Extensions.DependencyInjection;
using TfNet.Providers.Validation;

namespace TfNet.Registry;

internal class ServiceCollectionResourceRegisterer<T> : IResourceRegisterer<T>
{
    private readonly IServiceCollection _services;
    private readonly string _resourceName;

    public ServiceCollectionResourceRegisterer(
        IServiceCollection services,
        string resourceName)
    {
        _services = services;
        _resourceName = resourceName;
    }

    public IResourceRegisterer<T> WithDataAnnotationValidation()
    {
        _services.AddTransient<IValidationProvider<T>, DataAnnotationValidationProvider<T>>();
        return WithValidator<DataAnnotationValidationProvider<T>>();
    }

    public IResourceRegisterer<T> WithValidator<TValidator>() where TValidator : class, IValidationProvider<T>
    {
        _services.AddTransient<ValidationProviderHost<T>>();
        _services.AddSingleton(new ValidatorRegistryRegistration(_resourceName, typeof(T)));

        return this;
    }
}
