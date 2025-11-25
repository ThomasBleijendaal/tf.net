using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
using TfNet.Registry;
using TfNet.Schemas.Types;
using TfNet.Serialization;

namespace TfNet.Api;

public static class HostBuilderExtensions
{
    public const int DefaultGrpcPort = 5345;
    public const int DefaultHttpsPort = 5346;

    public static IHostBuilder AddTerraformPluginServices(
        this IHostBuilder builder,
        string fullProviderName,
        Action<IServiceCollection, IResourceRegistryContext> configure,
        int grpcPort = DefaultGrpcPort,
        int httpsPort = DefaultHttpsPort,
        string? routePrefix = null)
    {
        builder
            .ConfigureServices((host, services) =>
            {
                services.Configure<TerraformPluginHostOptions>(host.Configuration);
                services.Configure<TerraformPluginHostOptions>(x => x.FullProviderName = fullProviderName);
                services.AddSingleton(new PluginHostCertificate(
                    Certificate: CertificateGenerator.GenerateSelfSignedCertificate("CN=127.0.0.1", "CN=localhost", CertificateGenerator.GeneratePrivateKey())));
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(kestrel =>
                {
                    var debugMode = kestrel.ApplicationServices.GetRequiredService<IOptions<TerraformPluginHostOptions>>().Value.DebugMode;

                    if (debugMode)
                    {
                        kestrel.ListenLocalhost(httpsPort, x => x.Protocols = HttpProtocols.Http1);
                        kestrel.ListenLocalhost(grpcPort, x => x.UseHttps(x =>
                        {
                            var certificate = kestrel.ApplicationServices.GetService<PluginHostCertificate>()
                                ?? throw new InvalidOperationException("Debug mode is not enabled, but no certificate was found.");

                            x.ServerCertificate = certificate.Certificate;
                            x.AllowAnyClientCertificate();
                        }));
                    }
                });

                webBuilder.ConfigureServices(services =>
                {
                    services.AddOptions<TerraformPluginHostOptions>().ValidateDataAnnotations();
                    services.AddSingleton<ResourceRegistry>();

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

                    var registryContext = new ServiceCollectionResourceRegistryContext(services);

                    configure(services, registryContext);
                });

                webBuilder.Configure((context, app) =>
                {
                    var env = context.HostingEnvironment;

                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }

                    var apiKey = Environment.GetEnvironmentVariable("TF_PROXY_KEY")
                        ?? throw new InvalidOperationException("Missing TF_PROXY_KEY environment variable");

                    app.UseRouting();

                    app.Use((context, next) =>
                    {
                        var hasValidApiKeyHeader = context.Request.Headers.TryGetValue("x-tf-proxy-key", out var values)
                            && values.ToString() is string proxyKey
                            && proxyKey == apiKey;

                        var hasValidBasicAuthHeader = AuthenticationHeaderValue.TryParse(context.Request.Headers.Authorization.ToString(), out var value)
                            && value.Scheme == "Basic"
                            && value.Parameter is not null
                            && Encoding.UTF8.GetString(Convert.FromBase64String(value.Parameter)) is string userNamePassword
                            && userNamePassword.Split(":") is { Length: 2 } userNamePasswordSplit
                            && userNamePasswordSplit[1] == apiKey;

                        var hasValidAuth = hasValidApiKeyHeader || hasValidBasicAuthHeader;

                        if (!hasValidAuth)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }

                        return next(context);
                    });

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<Terraform6ProviderService>();

                        endpoints.MapGet(
                            $"{routePrefix}/.tf-proxy-schema",
                            async (HttpContext context, [FromServices] IOptions<TerraformPluginHostOptions> options) =>
                            {
                                await context.Response.WriteAsJsonAsync(options.Value);
                            });

                        endpoints.Map(
                            $"{routePrefix}/.tf-storage/{{stateId}}",
                            async (HttpContext context, string stateId, [FromServices] IStateStorage storage) =>
                            {
                                try
                                {
                                    var req = context.Request;

                                    var lockId = req.Query["id"].ToString();

                                    using var streamReader = new StreamReader(req.Body);
                                    var body = await streamReader.ReadToEndAsync();

                                    var lockInfo = !string.IsNullOrEmpty(lockId) || string.IsNullOrWhiteSpace(body)
                                        ? null
                                        : JsonSerializer.Deserialize<LockPayload>(body);

                                    var storageTask = req.Method switch
                                    {
                                        "GET" => storage.GetStateAsync(stateId),
                                        "POST" => storage.UpdateStateAsync(stateId, getLock(), getBody()),
                                        "DELETE" => storage.DeleteStateAsync(stateId),
                                        "UNLOCK" => storage.UnlockAsync(stateId, getLock()),
                                        "LOCK" => storage.LockAsync(stateId, getLock()),

                                        _ => throw new InvalidOperationException()
                                    };

                                    if (storageTask is Task<string?> responseTask)
                                    {
                                        var json = await responseTask;

                                        if (req.Method == "GET")
                                        {
                                            // fallback state for no state
                                            json = """"
                                                {
                                                    "version": 1
                                                }
                                                """";
                                        }

                                        return Results.Content(json, "application/json", Encoding.UTF8);
                                    }
                                    else if (storageTask is Task<bool> lockTask)
                                    {
                                        return Results.StatusCode(await lockTask ? StatusCodes.Status200OK : StatusCodes.Status423Locked);
                                    }
                                    else
                                    {
                                        await storageTask;
                                        return Results.Ok();
                                    }

                                    string getLock() => lockId ?? lockInfo?.Id ?? throw new InvalidOperationException("Invalid lock supplied");
                                    string getBody() => body ?? throw new InvalidOperationException("No body");
                                }
                                catch (Exception ex)
                                {
                                    return Results.InternalServerError(ex.Message);
                                }
                            });
                    });
                });
            });

        return builder;
    }
}

