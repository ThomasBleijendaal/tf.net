using Tfplugin6;

namespace TfNet.Schemas;

public interface IFunctionProvider
{
    /// <summary>
    /// Function name this FunctionProvider is associated with.
    /// </summary>
    string FunctionName { get; }

    /// <summary>
    /// Builds a Terraform schema based on its configuration. SchemaProviders are required to cache (parts of) their schema when applicable.
    /// </summary>
    ValueTask<Schema> GetSchemaAsync();
}
