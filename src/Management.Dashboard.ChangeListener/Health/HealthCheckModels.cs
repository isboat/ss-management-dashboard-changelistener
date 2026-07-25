using System.Text.Json.Serialization;

namespace Management.Dashboard.ChangeListener.Health;

public sealed record ComponentHealth(
    string Status,
    string? Description,
    long DurationMilliseconds);

public sealed record HealthCheckResponse(
    string Status,
    DateTimeOffset CheckedAt,
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
