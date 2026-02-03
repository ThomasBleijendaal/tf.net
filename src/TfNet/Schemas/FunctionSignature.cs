namespace TfNet.Schemas;

public record FunctionSignature
{
    public string? Summary { get; init; }

    public string? MarkdownDescription { get; init; }

    public required Dictionary<string, TypeInfo> Request { get; init; }

    public required Type Response { get; init; }

    public record TypeInfo(Type Type)
    {
        public string? MarkdownDescription { get; init; }
    }
}
