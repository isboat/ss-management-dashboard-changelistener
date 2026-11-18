# OnScreenSync dashboard change listener

The change listener is a small Azure Functions application that relays queued updates to connected dashboard clients. It lets a producer place a change message on a queue and lets the dashboard receive it in real time, without making each client listen to the queue itself.

## What it does

- Reads messages from the configured Azure Service Bus queue and forwards non-empty message bodies to the dashboard SignalR hub as notifications.
- Provides an HTTP negotiation endpoint clients use to obtain connection information for that hub.
- Reports the health of Azure Storage, the Service Bus queue, and Azure SignalR Service. The health response identifies which dependency needs attention.

## How it is organised

The [Functions project](../src/Management.Dashboard.ChangeListener) contains one function for queue broadcasting, one for connection negotiation, and one for reporting health. Host configuration and startup are in the same project. [Tests](../tests/Management.Dashboard.ChangeListener.Tests) exercise the listener independently of an Azure deployment. The [README](../README.md) lists the required settings and message contract.

## What it relies on

- **Azure Service Bus** is the input queue. **Azure SignalR Service** carries the forwarded notifications to compatible dashboard clients.
- **Azure Storage** supports the Functions host. **Application Insights** supports worker telemetry.
- **Azure Functions v4 and .NET 8** run the isolated worker. The repository uses the Azure Functions HTTP, Service Bus, and SignalR extensions for the function bindings.
