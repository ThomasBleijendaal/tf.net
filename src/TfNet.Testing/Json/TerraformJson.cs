using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TfNet.Testing.Json;

// See https://www.terraform.io/internals/json-format#plan-representation
public record TerraformJsonPlan
{
    [JsonPropertyName("format_version")]
    public string FormatVersion { get; init; } = null!;

    [JsonPropertyName("terraform_version")]
    public string TerraformVersion { get; init; } = null!;

    [JsonPropertyName("planned_values")]
    public TerraformPlannedValues PlannedValues { get; init; } = null!;

    [JsonPropertyName("output_changes")]
    public Dictionary<string, TerraformJsonChange> OutputChanges { get; init; } = null!;

    [JsonPropertyName("resource_changes")]
    public ImmutableList<TerraformJsonResourceChange> ResourceChanges { get; init; } = null!;

    [JsonPropertyName("prior_state")]
    public PriorState PriorState { get; init; } = null!;
}

public record TerraformJsonResourceChange
{
    [JsonPropertyName("address")]
    public string Address { get; init; } = null!;

    [JsonPropertyName("previous_address")]
    public string PreviousAddress { get; init; } = null!;

    [JsonPropertyName("module_address")]
    public string ModuleAddress { get; init; } = null!;

    [JsonPropertyName("mode")]
    public string Mode { get; init; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("deposed")]
    public string Deposed { get; init; } = null!;

    [JsonPropertyName("change")]
    public TerraformJsonChange Change { get; init; } = null!;

    [JsonPropertyName("action_reason")]
    public string ActionReason { get; init; } = null!;
}

// See https://www.terraform.io/internals/json-format#change-representation
public record TerraformJsonChange
{
    [JsonPropertyName("actions")]
    public ImmutableList<string> Actions { get; init; } = null!;

    [JsonPropertyName("before")]
    public JsonElement Before { get; init; }

    [JsonPropertyName("after")]
    public JsonElement After { get; init; }
}

public record PriorState
{
    [JsonPropertyName("format_version")]
    public string FormatVersion { get; init; } = null!;

    [JsonPropertyName("terraform_version")]
    public string TerraformVersion { get; init; } = null!;

    [JsonPropertyName("values")]
    public StateValues Values { get; init; } = null!;
}

public class StateValues
{
    [JsonPropertyName("root_module")]
    public Module RootModule { get; init; } = null!;
}

public class Module
{
    [JsonPropertyName("resources")]
    public ImmutableList<Resource> Resources { get; init; } = null!;
}

public class Resource
{
    [JsonPropertyName("address")]
    public string Address { get; init; } = null!;

    [JsonPropertyName("mode")]
    public string Mode { get; init; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("provider_name")]
    public string ProviderName { get; init; } = null!;

    [JsonPropertyName("schema_version")]
    public int SchemaVersion { get; init; }

    [JsonPropertyName("values")]
    public JsonElement Values { get; init; }

    [JsonPropertyName("sensitive_values")]
    public JsonElement SensitiveValues { get; init; }
}

public record TerraformPlannedValues
{
    [JsonPropertyName("outputs")]
    public Dictionary<string, TerraformPlannedValue> Outputs { get; init; } = null!;
}

public record TerraformPlannedValue
{
    [JsonPropertyName("sensitive")]
    public bool Sensitive { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;

    [JsonPropertyName("value")]
    public string Value { get; init; } = null!;
}
