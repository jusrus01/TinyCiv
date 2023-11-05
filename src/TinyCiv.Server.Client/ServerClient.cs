using TinyCiv.Shared;
using System.Text.Json;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using Microsoft.AspNetCore.Http.Connections.Client;

namespace TinyCiv.Server.Client;

// Client should use this as a singleton,
// unclear how WPF is used, so allowing creation of client without DI
public class ServerClient : IServerClient, IAsyncDisposable
{
    private readonly IHubConnection _hubConnection;

    private ServerClient(IHubConnection connection)
    {
        _hubConnection = connection;
    }

    private static IServerClient? _client;
    
    public static IServerClient Create(string hostUrl, Action<HttpConnectionOptions>? configureHttpConnection = null, bool createNewConnection = false)
    {
        if (_client != null && !createNewConnection)
        {
            return _client;
        }

        var connection = new SignalRAdapter(hostUrl, configureHttpConnection!);
        _client = new ServerClient(connection);

        return _client;
    }
    
    public Task SendAsync<T>(T @event, CancellationToken token = default) where T : ClientEvent
    {
        var content = JsonSerializer.Serialize(@event);
        return _hubConnection.SendAsync(Constants.Server.ReceiveFromClient, content, @event.Type, token);
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
        void callbackWrapper(ServerEvent @event) => callback((T)@event);
        _hubConnection.Listen(methodName, callbackWrapper);
    }

    public ValueTask DisposeAsync()
    {
        return _hubConnection.DisposeAsync();
    }
}