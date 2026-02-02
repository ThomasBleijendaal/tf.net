using Microsoft.Extensions.DependencyInjection;

namespace TfNet.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddAsyncInitializedSingleton<T>()
            where T : class, IAsyncInitialized
        {
            services.AddSingleton<T>();
            services.AddSingleton<IAsyncInitialized>(sp => sp.GetRequiredService<T>());
        }
    }
}
