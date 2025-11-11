namespace TfNet.Models;

public record PlanResult<T>(T Value)
{
    public List<AttributePath> RequiresReplace { get; init; } = [];
}
