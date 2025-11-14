using Microsoft.Extensions.DependencyInjection;
using TfNet.Api;
using TfNet.Providers.Data;
using TfNet.Providers.Function;
using TfNet.Providers.Resource;
using TfNet.Providers.Validation;
using TfNet.SampleCore;
using TfNet.SampleCore.DataSource;
using TfNet.SampleCore.Function;
using TfNet.SampleCore.Resource;

await TerraformApiHost.RunAsync(args, "example.com/example/sampleprovider", (services, registry) =>
{
    services.AddSingleton<SampleConfigurator>();

    services.AddSingleton<IValidationProvider<Configuration>, ConfigurationValidator>();
    services.AddSingleton<IValidationProvider<SampleFileResource>, SampleFileResourceValidator>();

    services.AddTerraformProviderConfigurator<Configuration, SampleConfigurator>()
        .WithValidator<ConfigurationValidator>();

    services.AddSingleton<IResourceProvider<SampleFileResource>, SampleFileResourceProvider>();
    registry.RegisterResource<SampleFileResource>("sampleprovider_file")
        .WithValidator<SampleFileResourceValidator>();

    services.AddSingleton<IDataSourceProvider<SampleFolderDataSource>, SampleFolderDataSourceProvider>();
    registry.RegisterDataSource<SampleFolderDataSource>("sampleprovider_folder");

    services.AddSingleton<IFunctionProvider<ConcatRequest, ConcatResponse>, ConcatFunction>();
    registry.RegisterFunction<ConcatRequest, ConcatResponse>("sampleprovider_concat");
});
