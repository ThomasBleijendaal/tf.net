using Tfplugin6;

namespace TfNet.Schemas;

public interface ISchemaBuilder
{
    /// <summary>
    /// Builds a Terraform schema for the specified CLR type.
    /// </summary>
    /// <param name="type">The type to build a schema for.</param>
    Schema BuildSchema(Type type);
}
