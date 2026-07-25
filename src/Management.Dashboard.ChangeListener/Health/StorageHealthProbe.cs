using Azure.Storage.Blobs;

namespace Management.Dashboard.ChangeListener.Health;

public sealed class StorageHealthProbe : IHealthProbe
{
    public string Name => "azureStorage";

    public async Task CheckAsync(CancellationToken cancellationToken)
    {
        var connectionString = GetRequiredSetting("AzureWebJobsStorage");
        var client = new BlobServiceClient(connectionString);
        await client.GetPropertiesAsync(cancellationToken);
    }

    private static string GetRequiredSetting(string name) =>
        Environment.GetEnvironmentVariable(name)
        ?? throw new HealthProbeException($"Required setting '{name}' is missing.");
}
