using System.ComponentModel;
using System.Reflection;
using Google.Protobuf;
using TfNet.Schemas.Types;
using Tfplugin6;
using KeyAttribute = MessagePack.KeyAttribute;

namespace TfNet.Schemas;

internal class FunctionSchemaProvider<TRequest, TResponse> : IFunctionProvider
{
    private readonly ITerraformTypeBuilder _typeBuilder;

    private Function? _function;

    public FunctionSchemaProvider(
        string functionName,
        ITerraformTypeBuilder typeBuilder)
    {
        _typeBuilder = typeBuilder;

        FunctionName = functionName;
    }

    public string FunctionName { get; }

    public ValueTask<Function> GetFunctionAsync()
    {
        if (_function != null)
        {
            return ValueTask.FromResult(_function);
        }


        var requestType = typeof(TRequest);
        var properties = requestType.GetProperties();

        var responseType = typeof(TResponse);
        var returnTerraformType = _typeBuilder.GetTerraformType(responseType);

        var function = new Function
        {
            Description = "TODO",
            Summary = "TODO",

            Return = new Function.Types.Return
            {
                Type = ByteString.CopyFromUtf8(returnTerraformType.ToJson())
            }
        };

        foreach (var property in properties)
        {
            var key = property.GetCustomAttribute<KeyAttribute>() ?? throw new InvalidOperationException($"Missing {nameof(KeyAttribute)} on {property.Name} in {requestType.Name}.");

            var description = property.GetCustomAttribute<DescriptionAttribute>();
            var required = TerraformTypeBuilder.IsRequiredAttribute(property);
            var terraformType = _typeBuilder.GetTerraformType(property.PropertyType);

            if (terraformType is TerraformType.TfObject _ && !required)
            {
                throw new InvalidOperationException("Optional object types are not supported.");
            }

            function.Parameters.Add(new Function.Types.Parameter
            {
                Name = key.StringKey,
                Type = ByteString.CopyFromUtf8(terraformType.ToJson()),
                Description = description?.Description ?? "",
                AllowNullValue = !required && !property.PropertyType.IsValueType,
                AllowUnknownValues = false,
                DescriptionKind = StringKind.Plain
            });
        }

        _function = function;

        return ValueTask.FromResult(_function);
    }
}
