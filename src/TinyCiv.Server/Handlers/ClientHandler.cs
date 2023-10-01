using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers;

public abstract class ClientHandler<TEvent> : IClientHandler where TEvent : ClientEvent
{
    private const string HandleMethodName = nameof(HandleAsync);

    private readonly ILogger<IClientHandler> _logger;

    private IClientProxy? _caller;
    private IClientProxy? _all;

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
        
        _caller = caller; // Simplest way to retrieve caller, trying to retrieve from DI is more complicated
        _all = all;

        var handlerName = GetType().Name;

        var @event = JsonSerializer.Deserialize<TEvent>(eventContent)!;
        _logger.LogInformation("{handler}.{method_name}: deserialized event data {@data}", handlerName, HandleMethodName, @event);

        if (IgnoreWhen(@event))
        {
            _logger.LogWarning("{handler}.{method_name}: ignored event data {@data}", handlerName, HandleMethodName, @event);
            return Task.CompletedTask;
        }

        try
        {
            _logger.LogInformation("{handler}.{method_name}: starting to process event '{@data}'", handlerName, HandleMethodName, @event);
            return OnHandleAsync(@event);
        }
        catch
        {
            _logger.LogError("{handler}.{method_name}: failed to process event '{@data}'", handlerName, HandleMethodName, @event);
            throw;
        }
    }

    protected Task NotifyCallerAsync<T>(string methodName, T serverEvent) where T : ServerEvent
    {
        ArgumentNullException.ThrowIfNull(_caller);
        _logger.LogInformation("{handler} is sending event {event_type} to caller", GetType().Name, serverEvent.Type);
        return InternalNotifyAsync(_caller, methodName, serverEvent);
    }
    
    protected Task NotifyAllAsync<T>(string methodName, T serverEvent) where T : ServerEvent
    {
        ArgumentNullException.ThrowIfNull(_all);
        _logger.LogInformation("{handler} is sending event {event_type} to all", GetType().Name, serverEvent.Type);
        return InternalNotifyAsync(_all, methodName, serverEvent);
    }

    // TODO: retry mechanism if after deployment some requests start to fail due to transient issues
    private Task InternalNotifyAsync<T>(IClientProxy proxy, string methodName, T serverEvent) where T : ServerEvent
    {
        return proxy
            .SendEventAsync(methodName, serverEvent)
            .ContinueWith(prevTask =>
            {
                if (prevTask.Exception == null)
                {
                    _logger.LogInformation("{handler} successfully notified client", GetType().Name);
                }
                else
                {
                    _logger.LogError(prevTask.Exception, "{handler} failed to notify client", GetType().Name);
                }

                return Task.CompletedTask;
            });
    }

    protected virtual bool IgnoreWhen(TEvent @event) => false;
        
    protected abstract Task OnHandleAsync(TEvent @event);
}