using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Management.Dashboard.ChangeListener;

public class QueueBroadcastFunction
{
    private readonly ILogger<QueueBroadcastFunction> _logger;

    public QueueBroadcastFunction(ILogger<QueueBroadcastFunction> logger)
    {
        _logger = logger;
    }

    [Function("queueBroadcast")]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public SignalRMessageAction Run(
        [ServiceBusTrigger("%QueueName%", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message)
    {
        var payload = message.Body.ToString();

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new InvalidOperationException(
                $"Service Bus message '{message.MessageId}' has an empty body and cannot be broadcast.");
        }

        _logger.LogInformation(
            "Broadcasting Service Bus message {MessageId} from sequence {SequenceNumber}. Payload length: {PayloadLength}",
            message.MessageId,
            message.SequenceNumber,
            payload.Length);

        return new SignalRMessageAction(SignalRConstants.NotifyTarget)
        {
            Arguments = [payload]
        };
    }
}
