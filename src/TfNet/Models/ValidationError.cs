namespace TfNet.Models;

public record ValidationError(string Message, AttributePath[] Paths);
