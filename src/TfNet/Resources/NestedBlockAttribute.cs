namespace TfNet.Resources;

/// <summary>
/// Indicates that a property is a nested block instead of a property
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NestedBlockAttribute : Attribute
{
    public int MinItems { get; set; } = 0;

    public int MaxItems { get; set; } = 1;
}
