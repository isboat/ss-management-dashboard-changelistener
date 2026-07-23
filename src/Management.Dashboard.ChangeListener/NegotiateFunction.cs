using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;

namespace Management.Dashboard.ChangeListener;

public static class NegotiateFunction
{
    [Function("negotiate")]
    public static SignalRConnectionInfo Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData _,
        [SignalRConnectionInfoInput(HubName = SignalRConstants.HubName)] SignalRConnectionInfo connectionInfo)
    {
        return connectionInfo;
    }
}
