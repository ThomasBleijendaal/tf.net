namespace TfNet.Schemas;

public record FunctionSignature(Dictionary<string, Type> Request, Type Response);
