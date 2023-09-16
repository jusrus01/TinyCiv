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

    public ServerHub(IEnumerable<IClientHandler> handlers)
    {
        _handlers = handlers;
    }
    
    [HubMethodName(Constants.Server.ReceiveFromClient)]
    public async Task ReceiveFromClient(string eventContent, string eventType)
    {
        // Potentially can be exported to factory pattern
        var handler = _handlers.SingleOrDefault(handler => handler.CanHandle(eventType));
        if (handler == null)
        {
            throw new NotSupportedException($"Event of type '{eventType}' not supported");
        }

        await handler
            .HandleAsync(Clients.Caller, Clients.All, eventContent)
            .ConfigureAwait(false);
    }
}