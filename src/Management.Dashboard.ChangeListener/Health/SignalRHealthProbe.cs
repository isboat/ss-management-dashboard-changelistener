namespace Management.Dashboard.ChangeListener.Health;

public sealed class SignalRHealthProbe : IHealthProbe
{
    public const string HttpClientName = "SignalRHealth";
    private readonly IHttpClientFactory _httpClientFactory;

    public SignalRHealthProbe(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public string Name => "azureSignalR";

    public async Task CheckAsync(CancellationToken cancellationToken)
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureSignalRConnectionString")
            ?? throw new HealthProbeException("Required setting 'AzureSignalRConnectionString' is missing.");
        var endpoint = ParseEndpoint(connectionString);
        var healthEndpoint = new Uri(endpoint, "/api/health");

        using var response = await _httpClientFactory.CreateClient(HttpClientName)
            .GetAsync(healthEndpoint, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private static Uri ParseEndpoint(string connectionString)
    {
        var endpointPart = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(part => part.TrimStart().StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase));
        var value = endpointPart?.Split('=', 2)[1];

        return Uri.TryCreate(value, UriKind.Absolute, out var endpoint)
            ? endpoint
            : throw new HealthProbeException("The Azure SignalR connection string has no valid endpoint.");
    }
}
