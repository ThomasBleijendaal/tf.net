using Tfplugin6;

namespace TfNet.Schemas;

public interface IFunctionSchemaProvider
{
    /// <summary>
    /// Function name this FunctionSchemaProvider is associated with.
    /// </summary>
    string FunctionName { get; }

    /// <summary>
    /// Builds a Terraform schema based on its configuration. FunctionSchemaProviders are required to cache (parts of) their schema when applicable.
    /// </summary>
    ValueTask<Function> GetFunctionSchemaAsync();

    /// <summary>
    /// Builds a parameter setter based on the schema. FunctionSchemaProviders are required to cache (parts of) their schema when applicable.
    /// </summary>
    /// <returns></returns>
    ValueTask<IParameterSetter> GetRequestSetterAsync();
}
