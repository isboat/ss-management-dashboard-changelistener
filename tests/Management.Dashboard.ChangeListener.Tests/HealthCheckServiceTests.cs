using System.Text.Json;
using Management.Dashboard.ChangeListener.Health;
using Microsoft.Extensions.Logging.Abstractions;

namespace Management.Dashboard.ChangeListener.Tests;

public sealed class HealthCheckServiceTests
{
    [Fact]
    public async Task CheckAsync_WhenEveryProbeSucceeds_ReturnsHealthyComponents()
    {
        var service = CreateService(
            new StubProbe("storage"),
            new StubProbe("queue"));

        var result = await service.CheckAsync(CancellationToken.None);

        Assert.True(result.IsHealthy);
        Assert.Equal("healthy", result.Status);
        Assert.All(result.Components.Values, component => Assert.Equal("healthy", component.Status));
    }

    [Fact]
    public async Task CheckAsync_WhenProbeReportsSafeFailure_ReturnsItsDescription()
    {
        var service = CreateService(
            new StubProbe("storage"),
            new StubProbe("queue", new HealthProbeException("The configured queue does not exist.")));

        var result = await service.CheckAsync(CancellationToken.None);

        Assert.False(result.IsHealthy);
        Assert.Equal("unhealthy", result.Status);
        Assert.Contains("queue", result.Components);
        var component = result.Components["queue"];
        Assert.Equal("unhealthy", component.Status);
        Assert.Equal("The configured queue does not exist.", component.Description);
    }

    [Fact]
    public async Task CheckAsync_WhenProbeThrowsUnexpectedFailure_DoesNotExposeExceptionMessage()
    {
        var service = CreateService(new StubProbe("storage", new Exception("secret connection string")));

        var result = await service.CheckAsync(CancellationToken.None);

        Assert.Equal("The dependency is unavailable.", result.Components["storage"].Description);
    }

    [Fact]
    public void Response_UsesDocumentedJsonPropertyNames()
    {
        var response = new HealthCheckResponse(
            "healthy",
            DateTimeOffset.UnixEpoch,
            new Dictionary<string, ComponentHealth>
            {
                ["storage"] = new("healthy", null, 12)
            });

        var json = JsonSerializer.Serialize(response);

        Assert.Contains("\"checkedAt\"", json);
        Assert.Contains("\"durationMilliseconds\"", json);
        Assert.DoesNotContain("\"CheckedAt\"", json);
    }

    private static HealthCheckService CreateService(params IHealthProbe[] probes) =>
        new(probes, NullLogger<HealthCheckService>.Instance);

    private sealed class StubProbe(string name, Exception? exception = null) : IHealthProbe
    {
        public string Name => name;

        public Task CheckAsync(CancellationToken cancellationToken) =>
            exception is null ? Task.CompletedTask : Task.FromException(exception);
    }
}
