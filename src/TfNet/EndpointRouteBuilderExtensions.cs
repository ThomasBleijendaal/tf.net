using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TfNet.Services;

namespace TfNet;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapTerraformPlugin(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGrpcService<Terraform6ProviderService>();
        return endpointRouteBuilder;
    }
}
