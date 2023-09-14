using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Shared;
using TinyCiv.Shared.Events;

namespace TinyCiv.Server.Hubs;

// This hub will send appropriate event for specific handlers,
// and each handler will receive a session object (much like HttpContext)
// where it will be able to reference it's state

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ServerHub : Hub
{
    [HubMethodName(Constants.Server.ReceiveFromClient)]
    public async Task ReceiveFromClient(SimpleEvent @event)
    {
        Console.WriteLine(@event.Message);
        
        await Clients.All.SendAsync(Constants.Server.SendToClient, JsonSerializer.Serialize(new SimpleEvent("Hello, client!")));
    }
}