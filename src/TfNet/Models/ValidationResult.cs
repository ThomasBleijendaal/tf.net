namespace TfNet.Models;

public class ValidationResult
{
    public static readonly ValidationResult? Success = null;

    public List<ValidationError> ValidationErrors { get; init; } = [];
}
