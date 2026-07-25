using System.Net;
using Management.Dashboard.ChangeListener.Health;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Management.Dashboard.ChangeListener;

public sealed class HealthFunction
{
    private readonly HealthCheckService _healthCheckService;

    public HealthFunction(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [Function("health")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData request,
        FunctionContext context)
    {
        var health = await _healthCheckService.CheckAsync(context.CancellationToken);
        var response = request.CreateResponse(
            health.IsHealthy ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable);
        await response.WriteAsJsonAsync(health, context.CancellationToken);
        return response;
    }
}
