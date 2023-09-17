using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Handlers;

public abstract class ClientHandler<TEvent> : IClientHandler where TEvent : ClientEvent
{
    public bool CanHandle(string type)
    {
        return type == typeof(TEvent).Name;
    }

    public Task HandleAsync(IClientProxy caller, IClientProxy all, string eventContent)
    {
        ArgumentNullException.ThrowIfNull(eventContent);

        var @event = JsonSerializer.Deserialize<TEvent>(eventContent)!;

        if (IgnoreWhen(@event))
        {
            return Task.CompletedTask;
        }
        
        return OnHandleAsync(caller, all, @event);
    }

    protected virtual bool IgnoreWhen(TEvent @event) => false;
        
    protected abstract Task OnHandleAsync(IClientProxy caller, IClientProxy all, TEvent @event);
}