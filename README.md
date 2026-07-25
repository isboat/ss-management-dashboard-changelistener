# ss-management-dashboard-changelistener

Azure Functions app that listens for management-dashboard change messages on Azure Service Bus and broadcasts them to connected dashboard clients with Azure SignalR Service.

## Architecture

```text
Azure Service Bus queue -> queueBroadcast function -> Azure SignalR Service -> dashboard clients
                                  ^
                                  |
                         negotiate HTTP endpoint
```

## Runtime

- .NET 8
- Azure Functions v4
- .NET isolated worker model
- Azure Service Bus trigger
- Azure SignalR Service input/output bindings
- Application Insights worker telemetry

## Functions

| Function | Trigger | Purpose |
| --- | --- | --- |
| `queueBroadcast` | Service Bus queue configured by `QueueName` | Broadcasts each queue message body to SignalR target `notify`. |
| `negotiate` | Anonymous HTTP `POST` | Returns SignalR connection information for `ChangeListenerHub`. |
| `health` | Anonymous HTTP `GET /api/health` | Checks Azure Storage, the configured Service Bus queue, and Azure SignalR Service. |

## Health check

`GET /api/health` returns HTTP `200` only when every dependency is available. It returns HTTP `503` when any check fails or the overall 10-second deadline expires. The JSON response includes the overall status, check time, and status and duration for every component:

```json
{
  "status": "healthy",
  "checkedAt": "2026-07-25T12:00:00+00:00",
  "components": {
    "azureStorage": { "status": "healthy", "description": null, "durationMilliseconds": 25 },
    "serviceBus": { "status": "healthy", "description": null, "durationMilliseconds": 31 },
    "azureSignalR": { "status": "healthy", "description": null, "durationMilliseconds": 18 }
  }
}
```

Dependency error details are logged but deliberately not returned, because Azure SDK errors may contain credentials. The endpoint is anonymous so a dashboard or platform probe can call it; use network access controls if dependency names themselves are sensitive.

## Required app settings

| Setting | Description |
| --- | --- |
| `AzureWebJobsStorage` | Storage account connection used by the Functions host. |
| `FUNCTIONS_WORKER_RUNTIME` | Must be `dotnet-isolated`. |
| `QueueName` | Name of the Service Bus queue to listen to. |
| `ServiceBusConnectionString` | Service Bus namespace connection string used by the queue trigger. |
| `AzureSignalRConnectionString` | Azure SignalR Service connection string used by the SignalR bindings. |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Optional but recommended for production telemetry. |

## Local development

1. Install the .NET 8 SDK and Azure Functions Core Tools v4.
2. Create `src/Management.Dashboard.ChangeListener/local.settings.json` with the required local app settings. Do not commit this file.
3. Start the app from `src/Management.Dashboard.ChangeListener`:

   ```bash
   func start --port 7110
   ```

## Message contract

The current broadcast contract preserves non-empty incoming Service Bus message bodies and sends the body as the first argument to the SignalR `notify` target.

Future changes should introduce an explicit JSON DTO and schema validation before broadcasting if the payload shape becomes owned by this service.

## Security notes

- The `negotiate` endpoint is currently anonymous. Keep it behind appropriate network, gateway, or platform authentication controls if the dashboard is not public.
- The queue processor logs message metadata and payload length only; it does not log message body contents.
