namespace TfNet.Schemas;

internal interface IDynamicFunctionSchemaProvider
{
    string FunctionNamePrefix { get; }

    IFunctionHandler Handler { get; }

    ValueTask<Dictionary<string, IFunctionSchemaProvider>> GetFunctionSchemasAsync();
}
