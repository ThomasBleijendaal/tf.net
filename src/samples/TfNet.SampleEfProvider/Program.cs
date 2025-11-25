using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TfNet.Plugin;
using TfNet.Providers.Resource;
using TfNet.SampleEfProvider.Data;
using TfNet.SampleEfProvider.Providers;

await TerraformPluginHost.RunAsync(args, "example.com/example/dbprovider", (services, registry) =>
{
    services.AddDbContext<ProviderDbContext>(
        options =>
        {
            options.UseSqlite(@"Data Source=C:\Projects\_prive\tf.net\src\samples\TfNet.SampleEfProvider\database.db");
        });

    services.AddScoped<IResourceProvider<UserResource>, UserResourceProvider>();
    registry.RegisterResource<UserResource>("dbprovider_user");

    services.AddScoped<IResourceProvider<UserRoleResource>, UserRoleResourceProvider>();
    registry.RegisterResource<UserRoleResource>("dbprovider_role");
});
