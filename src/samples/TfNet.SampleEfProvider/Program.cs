using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TfNet.Plugin;
using TfNet.Providers.Resource;
using TfNet.SampleEfProvider;
using TfNet.SampleEfProvider.Data;
using TfNet.SampleEfProvider.Providers;

await TerraformPluginHost.RunAsync(args, "example.com/example/dbprovider", (services, registry) =>
{
    services.AddDbContext<ProviderDbContext>(
        options => options.UseSqlite(@"Data Source=C:\Projects\_prive\tf.net\src\samples\TfNet.SampleEfProvider\database.db"));

    services.AddSingleton<SampleConfigurator>();

    //services.AddSingleton<IValidationProvider<Configuration>, ConfigurationValidator>();
    //services.AddSingleton<IValidationProvider<SampleFileResource>, SampleFileResourceValidator>();

    services.AddTerraformProviderConfigurator<Configuration, SampleConfigurator>();
    //    .WithValidator<ConfigurationValidator>();

    services.AddSingleton<IResourceProvider<UserResource>, UserResourceProvider>();
    registry.RegisterResource<UserResource>("dbprovider_user");
    //    .WithValidator<SampleFileResourceValidator>();

    //services.AddSingleton<IDataSourceProvider<SampleFolderDataSource>, SampleFolderDataSourceProvider>();
    //registry.RegisterDataSource<SampleFolderDataSource>("sampleprovider_folder");

    //services.AddSingleton<IFunctionProvider<ConcatRequest, ConcatResponse>, ConcatFunction>();
    //registry.RegisterFunction<ConcatRequest, ConcatResponse>("sampleprovider_concat");
});
