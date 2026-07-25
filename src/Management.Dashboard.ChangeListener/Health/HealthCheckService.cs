using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Management.Dashboard.ChangeListener.Health;

public sealed class HealthCheckService
{
    private static readonly TimeSpan OverallTimeout = TimeSpan.FromSeconds(10);
    private readonly IReadOnlyCollection<IHealthProbe> _probes;
    private readonly ILogger<HealthCheckService> _logger;

    public HealthCheckService(IEnumerable<IHealthProbe> probes, ILogger<HealthCheckService> logger)
    {
        _probes = probes.ToArray();
        _logger = logger;
    }

    public async Task<HealthCheckResponse> CheckAsync(CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(OverallTimeout);

        var results = await Task.WhenAll(_probes.Select(probe => CheckProbeAsync(probe, timeout.Token)));
        var components = results.ToDictionary(result => result.Name, result => result.Health);
        var status = components.Values.All(component => component.Status == HealthStatus.Healthy)
            ? HealthStatus.Healthy
            : HealthStatus.Unhealthy;

        return new HealthCheckResponse(status, DateTimeOffset.UtcNow, components);
    }

    private async Task<(string Name, ComponentHealth Health)> CheckProbeAsync(
        IHealthProbe probe,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await probe.CheckAsync(cancellationToken);
            return (probe.Name, new ComponentHealth(HealthStatus.Healthy, null, stopwatch.ElapsedMilliseconds));
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "Health check for {ComponentName} failed.", probe.Name);
            return (probe.Name, new ComponentHealth(
                HealthStatus.Unhealthy,
                Sanitize(exception.Message),
                stopwatch.ElapsedMilliseconds));
        }
        catch (OperationCanceledException)
        {
            return (probe.Name, new ComponentHealth(
                HealthStatus.Unhealthy,
                "The dependency check timed out.",
                stopwatch.ElapsedMilliseconds));
        }
    }

    private static string Sanitize(string message)
    {
        // Azure SDK error messages can echo connection strings. Return a stable public message instead.
        return string.IsNullOrWhiteSpace(message) ? "The dependency check failed." : "The dependency is unavailable or incorrectly configured.";
    }
}
