using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TfNet;
using TfNet.Proxy;
using Tfplugin6;

var url = Environment.GetEnvironmentVariable("TF_PROXY_URL");
if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
{
    Console.WriteLine("Missing TF_PROXY_URL");
    return 100;
}

var key = Environment.GetEnvironmentVariable("TF_PROXY_KEY");
if (string.IsNullOrEmpty(key))
{
    Console.WriteLine("Missing TF_RPOXY_KEY");
    return 101;
}

var httpClient = new HttpClient
{
    BaseAddress = uri,
    DefaultRequestVersion = HttpVersion.Version20,
    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
};
httpClient.DefaultRequestHeaders.Add("x-tf-proxy-key", key);

var config = await httpClient.GetFromJsonAsync<TerraformPluginHostOptions>(".tf-proxy-schema");
if (config == null)
{
    Console.WriteLine("No valid terraform config found after fetching TF_PROXY_URL");
    return 200;
}

await TerraformPluginHost.RunAsync(args, config.FullProviderName, (services, registry) =>
{
    services.AddHttpClient("tf", httpClient =>
    {
        httpClient.BaseAddress = uri;
        httpClient.DefaultRequestHeaders.Add("x-tf-proxy-key", key);
    });

    services.AddGrpcClient<Provider.ProviderClient>("tf", o =>
    {
        o.Address = uri;
    });
});

return 0;
