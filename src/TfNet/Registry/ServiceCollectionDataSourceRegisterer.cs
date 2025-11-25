using Microsoft.Extensions.DependencyInjection;
using TfNet.Providers.Validation;

namespace TfNet.Registry;

internal class ServiceCollectionDataSourceRegisterer<T> : IDataSourceRegisterer<T>
{
    private readonly IServiceCollection _services;
    private readonly string _resourceName;

    public ServiceCollectionDataSourceRegisterer(
        IServiceCollection services,
        string resourceName)
    {
        _services = services;
        _resourceName = resourceName;
    }

    public IDataSourceRegisterer<T> WithDataAnnotationValidation()
    {
        _services.AddTransient<IValidationProvider<T>, DataAnnotationValidationProvider<T>>();
        return WithValidator<DataAnnotationValidationProvider<T>>();
    }

    public IDataSourceRegisterer<T> WithValidator<TValidator>() where TValidator : class, IValidationProvider<T>
    {
        _services.AddTransient<ValidationProviderHost<T>>();
        _services.AddSingleton(new ValidatorRegistryRegistration(_resourceName, typeof(T)));

        return this;
    }
}
