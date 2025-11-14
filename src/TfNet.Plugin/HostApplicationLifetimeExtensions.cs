using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace TfNet.Plugin;

public static class HostApplicationLifetimeExtensions
{
    public static IHostApplicationLifetime InitializeTerraformPlugin(this IHostApplicationLifetime lifetime, IApplicationBuilder app, IOptions<TerraformPluginHostOptions> pluginHostOptions)
    {
        lifetime.ApplicationStarted.Register(() =>
        {
            var serverAddress = app.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses.First()
                ?? throw new InvalidOperationException($"{nameof(IServerAddressesFeature)} not found in {nameof(app.ServerFeatures)}");

            var serverUri = new Uri(serverAddress);
            var host = serverUri.Host == "localhost" ? "127.0.0.1" : serverUri.Host;

            if (pluginHostOptions.Value.DebugMode)
            {
                Console.WriteLine("Debug mode enabled (no certificate). Run Terraform with the following environment variable set:");

                var json = $@"{{""{pluginHostOptions.Value.FullProviderName}"":{{""Protocol"":""grpc"",""ProtocolVersion"":6,""Pid"":{Environment.ProcessId},""Test"":true,""Addr"":{{""Network"":""tcp"",""String"":""{host}:{serverUri.Port}""}}}}}}";
                Console.WriteLine($@"TF_REATTACH_PROVIDERS={json}");
                Console.WriteLine($@"$env:TF_REATTACH_PROVIDERS='{json}'");
            }
            else
            {
                var pluginHostCertificate = app.ApplicationServices.GetRequiredService<PluginHostCertificate>();

                // Terraform seems not to like Base64 padding, so we trim
                var base64EncodedCertificate = Convert.ToBase64String(pluginHostCertificate.Certificate.RawData).TrimEnd('=');
                Console.WriteLine($"1|6|tcp|{host}:{serverUri.Port}|grpc|{base64EncodedCertificate}");
            }
        });

        return lifetime;
    }
}
