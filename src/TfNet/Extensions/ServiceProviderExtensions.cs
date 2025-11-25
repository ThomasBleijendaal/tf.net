using Microsoft.Extensions.DependencyInjection;

namespace TfNet.Extensions;

internal static class ServiceProviderExtensions
{
    extension(IServiceProvider sp)
    {
        public T BuildService<T>(object[] args)
        {
            return ActivatorUtilities.CreateInstance<T>(sp, args);
        }
    }
}
