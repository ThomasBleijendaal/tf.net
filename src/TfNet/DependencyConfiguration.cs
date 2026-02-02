using Microsoft.Extensions.DependencyInjection;
using TfNet.Extensions;
using TfNet.Registry;

namespace TfNet;

public static class DependencyConfiguration
{
    extension(IServiceCollection services)
    {
        public void AddCoreServices()
        {
            services.AddAsyncInitializedSingleton<FunctionRegistry>();
            services.AddAsyncInitializedSingleton<ResourceRegistry>();
        }
    }
}
