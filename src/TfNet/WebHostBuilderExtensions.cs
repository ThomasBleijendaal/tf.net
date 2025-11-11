using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TfNet.ResourceProvider;

namespace TfNet;

public static class WebHostBuilderExtensions
{
    public const int DefaultPort = 5344;

    public static IWebHostBuilder ConfigureTerraformPlugin(this IWebHostBuilder webBuilder, Action<IServiceCollection, IResourceRegistryContext> configureRegistry, int port = DefaultPort)
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
            var registryContext = services.AddTerraformResourceRegistry();
            configureRegistry(services, registryContext);
        });

        return webBuilder;
    }
}
