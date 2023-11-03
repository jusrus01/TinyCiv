using System.Text.Json;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Client;

// Client should use this as a singleton,
// unclear how WPF is used, so allowing creation of client without DI
public class ServerClient : IServerClient, IAsyncDisposable
{
    private readonly HubConnection _connection;
    
    private ServerClient(HubConnection connection)
    {
        _connection = connection;
    }
    
    private static IServerClient? _client;
    
    public static IServerClient Create(string hostUrl, Action<HttpConnectionOptions>? configureHttpConnection = null, bool createNewConnection = false)
    {
        if (_client != null && !createNewConnection)
        {
            return _client;
        }
        
        // Please modify according to your needs e.g.
        // move this or configure other way
        var connection = new HubConnectionBuilder()
            .WithUrl($"{hostUrl}{Constants.Server.HubRoute}", configureHttpConnection!)
            .AddJsonProtocol()
            .Build();

        connection.StartAsync().Wait();

        _client = new ServerClient(connection);
        return _client;
    }
    
    public Task SendAsync<T>(T @event, CancellationToken token = default) where T : ClientEvent
    {
        var content = JsonSerializer.Serialize(@event);
        return _connection.SendAsync(Constants.Server.ReceiveFromClient, content, @event.Type, token);
    }

    public void ListenForUnitStatusUpdate(Action<UnitStatusUpdateServerEvent> callback)
    {
        Listen(Constants.Server.SendUnitStatusUpdate, callback);
    }

    public void ListenForNewUnitCreation(Action<CreateUnitServerEvent> callback)
    {
        Listen(Constants.Server.SendCreatedUnit, callback);
    }

    public void ListenForNewPlayerCreation(Action<JoinLobbyServerEvent> callback)
    {
        Listen(Constants.Server.SendCreatedPlayer, callback);
    }

    public void ListenForGameStart(Action<GameStartServerEvent> callback)
    {
        Listen(Constants.Server.SendGameStartToAll, callback);
    }

    public void ListenForMapChange(Action<MapChangeServerEvent> callback)
    {
        Listen(Constants.Server.SendMapChangeToAll, callback);
    }

    public void ListenForResourcesUpdate(Action<ResourcesUpdateServerEvent> callback)
    {
        Listen(Constants.Server.SendResourcesStatusUpdate, callback);
    }

    /// <summary>
    /// <see cref="callback"/> will be invoked when more than one player has joined
    /// the lobby. 
    /// </summary>
    public void ListenForLobbyState(Action<LobbyStateServerEvent> callback)
    {
        Listen(Constants.Server.SendLobbyStateToAll, callback);
    }

    public void ListenForInteractableObjectChanges(Action<InteractableObjectServerEvent> callback)
    {
        Listen(Constants.Server.SendInteractableObjectChangesToAll, callback);
    }

    public void ListenForVictoryEvent(Action<VictoryServerEvent> callback)
    {
        Listen(Constants.Server.SendVictoryEventToAll, callback);
    }

    public void ListenForDefeatEvent(Action<DefeatServerEvent> callback)
    {
        Listen(Constants.Server.SendDefeatEventToAll, callback);
    }

    private void Listen<T>(string methodName, Action<T> callback) where T : ServerEvent
    {
        _connection.On<string, string>(
            methodName,
            (content, type) =>
            {
                if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(type))
                {
                    return;
                }

                var @event = ResolveEvent(content, type);
                
                callback((T)@event);
            });
    }

    private static ServerEvent ResolveEvent(string content, string type)
    {
        if (type == nameof(JoinLobbyServerEvent))
        {
            return JsonSerializer.Deserialize<JoinLobbyServerEvent>(content)!;
        }

        if (type == nameof(GameStartServerEvent))
        {
            return JsonSerializer.Deserialize<GameStartServerEvent>(content)!;
        }
        
        if (type == nameof(MapChangeServerEvent))
        {
            return JsonSerializer.Deserialize<MapChangeServerEvent>(content)!;
        }

        if (type == nameof(CreateUnitServerEvent))
        {
            return JsonSerializer.Deserialize<CreateUnitServerEvent>(content)!;
        }

        if (type == nameof(UnitStatusUpdateServerEvent))
        {
            return JsonSerializer.Deserialize<UnitStatusUpdateServerEvent>(content)!;
        }

        if (type == nameof(LobbyStateServerEvent))
        {
            return JsonSerializer.Deserialize<LobbyStateServerEvent>(content)!;
        }

        if (type == nameof(ResourcesUpdateServerEvent))
        {
            return JsonSerializer.Deserialize<ResourcesUpdateServerEvent>(content)!;
        }

        if (type == nameof(InteractableObjectServerEvent))
        {
            return JsonSerializer.Deserialize<InteractableObjectServerEvent>(content)!;
        }

        if (type == nameof(VictoryServerEvent))
        {
            return JsonSerializer.Deserialize<VictoryServerEvent>(content)!;
        }
        
        if (type == nameof(DefeatServerEvent))
        {
            return JsonSerializer.Deserialize<DefeatServerEvent>(content)!;
        }

        throw new NotSupportedException();
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}