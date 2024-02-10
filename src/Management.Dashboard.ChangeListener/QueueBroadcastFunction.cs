using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Management.Dashboard.ChangeListener
{
    public class QueueBroadcastFunction
    {
        [FunctionName("queueBroadcast")]
        public void Run(
            [ServiceBusTrigger("%QueueName%", Connection = "ServiceBusConnectionString")]string myQueueItem, 
            ILogger log,
            [SignalR(HubName = "ChangeListenerHub")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            log.LogInformation($"C# processed message: {myQueueItem}"); 

            signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "notify",
                    Arguments = new[]
                    {
                        myQueueItem
                    }
                });
        }
    }
}
