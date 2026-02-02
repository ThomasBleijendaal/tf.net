using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using TfNet.PluginCore.Services;
using TfNet.Providers.Data;
using TfNet.Providers.Function;
using TfNet.Providers.ProviderConfig;
using TfNet.Providers.Resource;
using TfNet.Providers.ResourceUpgrade;
using TfNet.Schemas.Types;
using TfNet.Serialization;

namespace TfNet.Plugin;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();

        services.AddTransient<ITerraformTypeBuilder, TerraformTypeBuilder>();
        services.AddTransient(typeof(ProviderConfigurationHost<>));
        services.AddTransient(typeof(ResourceProviderHost<>));
        services.AddTransient(typeof(DataSourceProviderHost<>));
        services.AddTransient(typeof(FunctionProviderHost<,>));
        services.AddTransient(typeof(IResourceUpgrader<>), typeof(DefaultResourceUpgrader<>));
        services.AddTransient<IDynamicValueSerializer, DefaultDynamicValueSerializer>();

        var otel = services.AddOpenTelemetry();

        // TODO: make configurable
        otel.WithTracing(x =>
        {
            x.AddAspNetCoreInstrumentation();
            x.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://127.0.0.1:5341/ingest/otlp/v1/traces");
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                opt.Headers = "X-Seq-ApiKey=-";
            });
        });
    }

    public void Configure(
        IApplicationBuilder app,
        IHostApplicationLifetime lifetime,
        IWebHostEnvironment env,
        IOptions<TerraformPluginHostOptions> pluginHostOptions)
    {
        lifetime.InitializeTerraformPlugin(app, pluginHostOptions);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<Terraform6ProviderService>();

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }


}
