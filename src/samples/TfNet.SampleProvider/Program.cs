using Microsoft.Extensions.DependencyInjection;
using TfNet;
using TfNet.Providers.Data;
using TfNet.Providers.Function;
using TfNet.Providers.Resource;
using TfNet.Providers.Validation;
using TfNet.SampleProvider;
using TfNet.SampleProvider.DataSource;
using TfNet.SampleProvider.Function;
using TfNet.SampleProvider.Resource;

class Program
{
    static Task Main(string[] args)
    {
        // Use the default plugin host that takes care of certificates and hosting the Grpc services.

        return TerraformPluginHost.RunAsync(args, "example.com/example/sampleprovider", (services, registry) =>
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
    }
}
