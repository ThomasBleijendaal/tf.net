namespace TfNet.Models;

public record EphemeralResult
{
    public required DateTimeOffset RenewAt { get; init; }
}

public record EphemeralResult<T> : EphemeralResult
{
    public required T Value { get; init; }
}
