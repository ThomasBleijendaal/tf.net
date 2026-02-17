using Microsoft.Extensions.DependencyInjection;
using TfNet.Plugin;
using TfNet.Providers.Data;
using TfNet.Providers.EphemeralResource;
using TfNet.Providers.Function;
using TfNet.Providers.Resource;
using TfNet.Providers.Validation;
using TfNet.SampleCore;
using TfNet.SampleCore.DataSource;
using TfNet.SampleCore.DynamicFunction;
using TfNet.SampleCore.Ephemeral;
using TfNet.SampleCore.Function;
using TfNet.SampleCore.Resource;

await TerraformPluginHost.RunAsync(args, "example.com/example/sampleprovider", (services, registry) =>
{
    services.AddSingleton<SampleConfigurator>();

    services.AddSingleton<IValidationProvider<Configuration>, ConfigurationValidator>();

    services.AddTerraformProviderConfigurator<Configuration, SampleConfigurator>()
        .WithValidator<ConfigurationValidator>();

    services.AddSingleton<IValidationProvider<SampleFileResource>, SampleFileResourceValidator>();
    services.AddSingleton<IResourceProvider<SampleFileResource>, SampleFileResourceProvider>();
    registry.RegisterResource<SampleFileResource>("sampleprovider_file")
        .WithValidator<SampleFileResourceValidator>();

    services.AddSingleton<IDataSourceProvider<SampleFolderDataSource>, SampleFolderDataSourceProvider>();
    registry.RegisterDataSource<SampleFolderDataSource>("sampleprovider_folder");

    services.AddSingleton<IValidationProvider<PasswordEphemeralResource>, PasswordEphemeralResourceValidator>();
    services.AddSingleton<IEphemeralResourceProvider<PasswordEphemeralResource>, PasswordEphemeralResourceProvider>();
    registry.RegisterEphemeralResource<PasswordEphemeralResource>("sampleprovider_password")
        .WithValidator<PasswordEphemeralResourceValidator>();

    services.AddSingleton<IFunctionProvider<ConcatRequest, ConcatResponse>, ConcatFunction>();
    registry.RegisterFunction<ConcatRequest, ConcatResponse>("sampleprovider_concat");

    services.AddSingleton<DynamicFunctionProvider>();
    registry.RegisterFunctions<DynamicFunctionProvider>("sampleprovider_");
});
