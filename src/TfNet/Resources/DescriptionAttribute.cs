namespace TfNet.Resources;

/// <summary>
/// Indicates that a value is "known after apply".
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string markdownDescription)
    {
        MarkdownDescription = markdownDescription;
    }

    public string MarkdownDescription { get; set; }
}
