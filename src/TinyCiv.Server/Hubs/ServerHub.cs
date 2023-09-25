using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Shared;

namespace TinyCiv.Server.Hubs;

// This hub will send appropriate event for specific handlers,
// and each handler will receive a session object (much like HttpContext)
// where it will be able to reference it's state
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ServerHub : Hub
{
    private readonly IEnumerable<IClientHandler> _handlers;
    private readonly ILogger<ServerHub> _logger;
    
    public ServerHub(IEnumerable<IClientHandler> handlers, ILogger<ServerHub> logger)
    {
        _handlers = handlers;
        _logger = logger;
    }
    
    [HubMethodName(Constants.Server.ReceiveFromClient)]
    public async Task ReceiveFromClient(string eventContent, string eventType)
    {
        _logger.LogInformation("Call to {method_name} received '{event}'", nameof(ReceiveFromClient), eventContent);
        
        // Potentially can be exported to factory pattern
        var handler = _handlers.SingleOrDefault(handler => handler.CanHandle(eventType));
        if (handler == null)
        {
            _logger.LogError("Unable to find handler for event type '{type}'", eventType);
            throw new NotSupportedException($"Event of type '{eventType}' not supported");
        }
        
        await handler
            .HandleAsync(Clients.Caller, Clients.All, eventContent)
            .ConfigureAwait(false);
        _logger.LogInformation("Handler '{handler}' successfully finished processing event '{event}'", handler.GetType().Name, eventContent);
    }
}