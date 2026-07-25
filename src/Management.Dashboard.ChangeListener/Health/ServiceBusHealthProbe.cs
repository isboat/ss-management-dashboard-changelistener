using Azure.Messaging.ServiceBus.Administration;

namespace Management.Dashboard.ChangeListener.Health;

public sealed class ServiceBusHealthProbe : IHealthProbe
{
    public string Name => "serviceBus";

    public async Task CheckAsync(CancellationToken cancellationToken)
    {
        var connectionString = GetRequiredSetting("ServiceBusConnectionString");
        var queueName = GetRequiredSetting("QueueName");
        var client = new ServiceBusAdministrationClient(connectionString);

        if (!await client.QueueExistsAsync(queueName, cancellationToken))
        {
            throw new InvalidOperationException("The configured Service Bus queue does not exist.");
        }
    }

    private static string GetRequiredSetting(string name) =>
        Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException($"Required setting '{name}' is missing.");
}
