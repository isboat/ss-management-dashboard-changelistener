using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Management.Dashboard.ChangeListener.Health;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        services.AddHttpClient(SignalRHealthProbe.HttpClientName, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        services.AddSingleton<IHealthProbe, StorageHealthProbe>();
        services.AddSingleton<IHealthProbe, ServiceBusHealthProbe>();
        services.AddSingleton<IHealthProbe, SignalRHealthProbe>();
        services.AddSingleton<HealthCheckService>();
    })
    .Build();

host.Run();
