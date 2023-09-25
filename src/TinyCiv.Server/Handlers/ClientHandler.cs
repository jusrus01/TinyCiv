using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Handlers;

public abstract class ClientHandler<TEvent> : IClientHandler where TEvent : ClientEvent
{
    private const string HandleMethodName = nameof(HandleAsync);
    
    private readonly ILogger<IClientHandler> _logger;

    protected ClientHandler(ILogger<IClientHandler> logger)
    {
        _logger = logger;
    }
    
    public bool CanHandle(string type)
    {
        return type == typeof(TEvent).Name;
    }

    public Task HandleAsync(IClientProxy caller, IClientProxy all, string eventContent)
    {
        ArgumentNullException.ThrowIfNull(eventContent);

        var handlerName = GetType().Name;

        var @event = JsonSerializer.Deserialize<TEvent>(eventContent)!;
        _logger.LogInformation("{handler}.{method_name}: deserialized event data {@data}", handlerName, HandleMethodName, @event);

        if (IgnoreWhen(@event))
        {
            _logger.LogWarning("{handler}.{method_name}: ignored", handlerName, HandleMethodName);
            return Task.CompletedTask;
        }

        try
        {
            _logger.LogInformation("{handler}.{method_name}: starting to process event '{@data}'", handlerName, HandleMethodName, @event);
            return OnHandleAsync(caller, all, @event);
        }
        catch
        {
            _logger.LogError("{handler}.{method_name}: failed to process event '{@data}'", handlerName, HandleMethodName, @event);
            throw;
        }
    }

    protected virtual bool IgnoreWhen(TEvent @event) => false;
        
    protected abstract Task OnHandleAsync(IClientProxy caller, IClientProxy all, TEvent @event);
}