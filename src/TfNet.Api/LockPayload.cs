using System.Text.Json.Serialization;

namespace TfNet.Api;

internal class LockPayload
{
    [JsonPropertyName("ID")]
    public string? Id { get; init; }
}

