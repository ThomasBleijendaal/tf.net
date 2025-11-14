using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TfNet;
using TfNet.Proxy;
using Tfplugin6;

var url = Environment.GetEnvironmentVariable("TF_PROXY_URL");
if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
{
    uri = new Uri("https://localhost:5000");
}

var httpClient = new HttpClient
{
    BaseAddress = uri,
    DefaultRequestVersion = HttpVersion.Version20,
    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
};

var config = await httpClient.GetFromJsonAsync<TerraformPluginHostOptions>("/");

if (config == null)
{
    Console.WriteLine("Incorrect config");
    return -1;
}

await TerraformPluginHost.RunAsync(args, config.FullProviderName, (services, registry) =>
{
    services.AddGrpcClient<Provider.ProviderClient>(o => o.Address = uri);
});

return 0;
