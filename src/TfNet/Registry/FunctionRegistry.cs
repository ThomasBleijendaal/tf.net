using TfNet.Extensions;
using TfNet.Providers.Function;
using TfNet.Schemas;
using Tfplugin6;

namespace TfNet.Registry;

internal class FunctionRegistry : IAsyncInitialized
{
    private readonly IEnumerable<IDynamicFunctionSchemaProvider> _dynamicFunctionProviders;

    private readonly List<Registration<Function>> _functions = new();
    private readonly Dictionary<string, IFunctionRegistration> _functionRegistrations;
    private readonly Dictionary<string, IFunctionSchemaProvider> _functionProviders;

    public FunctionRegistry(
        IEnumerable<IFunctionSchemaProvider> functionProviders,
        IEnumerable<FunctionRegistryRegistration> functionRegistrations,
        IEnumerable<IDynamicFunctionSchemaProvider> dynamicFunctionProviders)
    {
        _dynamicFunctionProviders = dynamicFunctionProviders;

        _functionRegistrations = functionRegistrations.ToDictionary(x => x.ResourceName, x => (IFunctionRegistration)x);
        _functionProviders = functionProviders.ToDictionary(x => x.FunctionName);
    }

    async Task IAsyncInitialized.InitializeAsync()
    {
        foreach (var functionProvider in _functionProviders.Values)
        {
            var function = await functionProvider.GetFunctionSchemaAsync();

            _functions.Add(new(functionProvider.FunctionName, function));
        }

        foreach (var provider in _dynamicFunctionProviders)
        {
            var functions = await provider.GetFunctionSchemasAsync();

            foreach (var (name, (signature, functionProvider)) in functions)
            {
                _functionProviders[name] = functionProvider;

                var handlerFunctionName = name.Replace(provider.FunctionNamePrefix, "");

                _functionRegistrations[name] = new DynamicFunctionRegistryRegistration(
                    name,
                    handlerFunctionName,
                    signature,
                    provider.Handler);

                var function = await functionProvider.GetFunctionSchemaAsync();

                _functions.Add(new(name, function));
            }
        }
    }

    public IReadOnlyList<Registration<Function>> GetFunctions() => _functions;

    public async Task<IFunctionProviderHost?> GetFunctionProviderAsync(IServiceProvider sp, string name)
    {
        if (!_functionRegistrations.TryGetValue(name, out var registration))
        {
            return null;
        }

        if (registration is FunctionRegistryRegistration function)
        {
            return sp.GetService(typeof(FunctionProviderHost<,>).MakeGenericType(function.Request, function.Response)) as IFunctionProviderHost;
        }
        else if (registration is DynamicFunctionRegistryRegistration dynamicFunction)
        {
            return sp.BuildService<DynamicFunctionProviderHost>([dynamicFunction]);
        }
        else
        {
            throw new NotSupportedException("FunctionRegistration not supported");
        }
    }

    public async ValueTask<IParameterSetter?> GetFunctionRequestSetterAsync(string name)
        => _functionProviders.TryGetValue(name, out var functionProvider)
            ? (await functionProvider.GetRequestSetterAsync())
            : null;
}
