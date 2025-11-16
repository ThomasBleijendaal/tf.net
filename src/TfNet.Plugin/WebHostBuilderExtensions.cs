using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TfNet.Registry;

namespace TfNet.Plugin;

public static class WebHostBuilderExtensions
{
    public const int DefaultPort = 5344;

    extension(IWebHostBuilder webBuilder)
    {
        public IWebHostBuilder ConfigureTerraformPlugin(Action<IServiceCollection, IResourceRegistryContext> configureRegistry, int port = DefaultPort)
        {
            webBuilder.ConfigureKestrel(kestrel =>
            {
                var debugMode = kestrel.ApplicationServices.GetRequiredService<IOptions<TerraformPluginHostOptions>>().Value.DebugMode;

                if (debugMode)
                {
                    kestrel.ListenLocalhost(port, x => x.Protocols = HttpProtocols.Http2);
                }
                else
                {
                    kestrel.ListenLocalhost(port, x => x.UseHttps(x =>
                    {
                        var certificate = kestrel.ApplicationServices.GetService<PluginHostCertificate>()
                            ?? throw new InvalidOperationException("Debug mode is not enabled, but no certificate was found.");

                        x.ServerCertificate = certificate.Certificate;
                        x.AllowAnyClientCertificate();
                    }));
                }
            });

            webBuilder.ConfigureLogging((context, logging) =>
            {
                var options = context.Configuration.Get<TerraformPluginHostOptions>();

                if (options?.DebugMode != true)
                {
                    logging.SetMinimumLevel(LogLevel.Error);
                }
            });

            webBuilder.UseStartup<Startup>();

            webBuilder.ConfigureServices(services =>
            {
                services.AddOptions<TerraformPluginHostOptions>().ValidateDataAnnotations();
                services.AddSingleton<ResourceRegistry>();

                var registryContext = new ServiceCollectionResourceRegistryContext(services);

                try
                {
                    configureRegistry(services, registryContext);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            });

            return webBuilder;
        }
    }
}
