using System.Text.Json.Serialization;

namespace Management.Dashboard.ChangeListener.Health;

public sealed record ComponentHealth(
    [property: JsonPropertyName("status")]
    string Status,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("durationMilliseconds")]
    long DurationMilliseconds);

public sealed record HealthCheckResponse(
    [property: JsonPropertyName("status")]
    string Status,
    [property: JsonPropertyName("checkedAt")]
    DateTimeOffset CheckedAt,
    [property: JsonPropertyName("components")]
    IReadOnlyDictionary<string, ComponentHealth> Components)
{
    [JsonIgnore]
    public bool IsHealthy => Components.Values.All(component => component.Status == HealthStatus.Healthy);
}

internal static class HealthStatus
{
    public const string Healthy = "healthy";
    public const string Unhealthy = "unhealthy";
}
