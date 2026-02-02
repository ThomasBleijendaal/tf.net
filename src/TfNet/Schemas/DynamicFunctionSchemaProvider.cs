using Google.Protobuf;
using TfNet.Schemas.Types;
using Tfplugin6;

namespace TfNet.Schemas;

internal class DynamicFunctionSchemaProvider<TFunctionHandler> : IDynamicFunctionSchemaProvider
    where TFunctionHandler : IFunctionHandler
{
    private readonly ITerraformTypeBuilder _typeBuilder;

    public DynamicFunctionSchemaProvider(
        string functionNamePrefix,
        TFunctionHandler functionHandler,
        ITerraformTypeBuilder typeBuilder)
    {
        FunctionNamePrefix = functionNamePrefix;
        Handler = functionHandler;
        _typeBuilder = typeBuilder;
    }

    public string FunctionNamePrefix { get; }

    public IFunctionHandler Handler { get; }

    public async ValueTask<Dictionary<string, IFunctionSchemaProvider>> GetFunctionSchemasAsync()
    {
        var result = new Dictionary<string, IFunctionSchemaProvider>();

        var functions = await Handler.GetFunctionsAsync();

        foreach (var (name, signature) in functions)
        {
            result[$"{FunctionNamePrefix}{name}"] = new FunctionSignatureSchemaProvider(name, signature, _typeBuilder);
        }

        return result;
    }

    private sealed class FunctionSignatureSchemaProvider : IFunctionSchemaProvider
    {
        private readonly FunctionSignature _functionSignature;
        private readonly ITerraformTypeBuilder _typeBuilder;

        public FunctionSignatureSchemaProvider(
            string functionName,
            FunctionSignature functionSignature,
            ITerraformTypeBuilder typeBuilder)
        {
            FunctionName = functionName;
            _functionSignature = functionSignature;
            _typeBuilder = typeBuilder;
        }

        public string FunctionName { get; }

        public ValueTask<Function> GetFunctionSchemaAsync()
        {
            var returnTerraformType = _typeBuilder.GetTerraformType(_functionSignature.Response);

            var function = new Function
            {
                // TODO: fix
                Description = "TODO",
                Summary = "TODO",

                Return = new Function.Types.Return
                {
                    Type = ByteString.CopyFromUtf8(returnTerraformType.ToJson())
                }
            };

            foreach (var (name, type) in _functionSignature.Request)
            {
                var terraformType = _typeBuilder.GetTerraformType(type);

                function.Parameters.Add(new Function.Types.Parameter
                {
                    Name = name,
                    Type = ByteString.CopyFromUtf8(terraformType.ToJson()),
                    Description = "",
                    AllowNullValue = !type.IsValueType,
                    AllowUnknownValues = false
                });
            }

            return ValueTask.FromResult(function);
        }

        public ValueTask<IParameterSetter> GetRequestSetterAsync()
            => ValueTask.FromResult<IParameterSetter>(new FunctionSignatureSetter(_functionSignature));

    }
}
