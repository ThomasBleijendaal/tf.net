using Tfplugin6;

namespace TfNet.Schemas;

public interface ISchemaProvider
{
    /// <summary>
    /// Schema name this SchemaProvider is associated with.
    /// </summary>
    string SchemaName { get; }

    /// <summary>
    /// Schema type this SchemaProvider is associated with.
    /// </summary>
    SchemaType Type { get; }

    /// <summary>
    /// Builds a Terraform schema based on its configuration. SchemaProviders are required to cache (parts of) their schema when applicable.
    /// </summary>
    ValueTask<Schema> GetSchemaAsync();
}
