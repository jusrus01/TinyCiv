using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Handlers.Client;

public abstract class ClientHandler<TEvent> : IClientHandler where TEvent : ClientEvent
{
    public bool CanHandle(string type)
    {
        return type == typeof(TEvent).Name;
    }

    public Task HandleAsync(IClientProxy caller, IClientProxy all, string eventContent)
    {
        ArgumentNullException.ThrowIfNull(eventContent);
        
        return OnHandleAsync(caller, all, JsonSerializer.Deserialize<TEvent>(eventContent)!);
    }

    protected abstract Task OnHandleAsync(IClientProxy caller, IClientProxy all, TEvent @event);
}