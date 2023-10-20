using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client.Lobby;

namespace TinyCiv.Server.Hubs;

// This hub will send appropriate event for specific handlers,
// and each handler will receive a session object (much like HttpContext)
// where it will be able to reference it's state
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ServerHub : Hub
{
    private readonly IEnumerable<IClientHandler> _handlers;
    private readonly ILogger<ServerHub> _logger;
    private readonly IConnectionIdAccessor _accessor;
    private readonly IPublisher _publisher;

    public ServerHub(
        IEnumerable<IClientHandler> handlers,
        IConnectionIdAccessor accessor,
        IPublisher publisher,
        ILogger<ServerHub> logger)
    {
        _handlers = handlers;
        _logger = logger;
        _accessor = accessor;
        _publisher = publisher;
    }
    
    [HubMethodName(Constants.Server.ReceiveFromClient)]
    public async Task ReceiveFromClient(string eventContent, string eventType)
    {
        // Deprecated
        _accessor.Init(Context);

        var subscriber = new Subscriber(Context.ConnectionId, Clients.Caller);
        _publisher.Subscribe(subscriber);
        
        _logger.LogInformation("Call to {method_name} received '{event}'", nameof(ReceiveFromClient), eventContent);
        
        // Potentially can be exported to factory pattern
        var handler = ResolveHandler(eventType);

        await handler
            .HandleAsync(subscriber, eventContent)
            .ConfigureAwait(false);
        _logger.LogInformation("Handler '{handler}' successfully finished processing event '{event}'", handler.GetType().Name, eventContent);
    }

    private IClientHandler ResolveHandler(string eventType)
    {
        var handler = _handlers.SingleOrDefault(handler => handler.CanHandle(eventType));
        if (handler == null)
        {
            _logger.LogError("Unable to find handler for event type '{type}'", eventType);
            throw new NotSupportedException($"Event of type '{eventType}' not supported");
        }

        return handler;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _accessor.Init(Context);
        _logger.LogInformation("Call to {method_name} received from {connection_id}", nameof(OnDisconnectedAsync), _accessor.ConnectionId);
        
        var ungracefulLeaveLobbyEvent = new LeaveLobbyClientEvent();
        var subscriber = new Subscriber(Context.ConnectionId, Clients.Caller);
        
        var handler = ResolveHandler(ungracefulLeaveLobbyEvent.Type);
        try
        {
            _logger.LogInformation("{method_name}: initiating leave lobby event for '{connection_id}'",
                nameof(OnDisconnectedAsync), _accessor.ConnectionId);

            await handler
                .HandleAsync(subscriber, JsonSerializer.Serialize(ungracefulLeaveLobbyEvent))
                .ConfigureAwait(false);

            _logger.LogInformation("{method_name}: successfully left lobby '{connection_id}'",
                nameof(OnDisconnectedAsync), _accessor.ConnectionId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{method_name}: failed to leave lobby", nameof(OnDisconnectedAsync));
        }
        finally
        {
            _publisher.Unsubscribe(subscriber);
        }
    }
}