using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Management.Dashboard.ChangeListener;

public class QueueBroadcastFunction
{
    private const int PayloadPreviewLength = 128;
    private readonly ILogger<QueueBroadcastFunction> _logger;

    public QueueBroadcastFunction(ILogger<QueueBroadcastFunction> logger)
    {
        _logger = logger;
    }

    [Function("queueBroadcast")]
    [SignalROutput(HubName = "ChangeListenerHub")]
    public SignalRMessageAction Run(
        [ServiceBusTrigger("%QueueName%", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message)
    {
        var payload = message.Body.ToString();

        _logger.LogInformation(
            "Broadcasting Service Bus message {MessageId} from sequence {SequenceNumber}. Payload length: {PayloadLength}. Preview: {PayloadPreview}",
            message.MessageId,
            message.SequenceNumber,
            payload.Length,
            GetPayloadPreview(payload));

        return new SignalRMessageAction("notify")
        {
            Arguments = [payload]
        };
    }

    private static string GetPayloadPreview(string payload)
    {
        return payload.Length <= PayloadPreviewLength
            ? payload
            : string.Concat(payload.AsSpan(0, PayloadPreviewLength), "...");
    }
}
