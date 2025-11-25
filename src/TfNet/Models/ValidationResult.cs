namespace TfNet.Models;

public class ValidationResult
{
    public static ValidationResult? Success = null;

    public List<ValidationError> ValidationErrors { get; init; } = [];
}
