using TinyCiv.Shared;
using System.Text.Json;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace TinyCiv.Server.Client;

public class ServerClientV2 : IServerClient, IAsyncDisposable
{
    private HubConnection? _connection;
    
    public async Task ConnectAsync(string hostUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{hostUrl}{Constants.Server.HubRoute}")
            .AddJsonProtocol()
            .Build();

        var retryCount = 0;
        while (true)
        {
            try
            {
                await _connection.StartAsync();
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to make a connection, reason: {e.Message}. Retrying ({retryCount++})");
            }
        }
    }
    
    public Task SendAsync<T>(T @event, CancellationToken token = default) where T : ClientEvent
    {
        ArgumentNullException.ThrowIfNull(_connection);

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

    public void ListenForGameModeChangeEvent(Action<GameModeChangeServerEvent> callback)
    {
        Listen(Constants.Server.SendGameModeChangeEventToAll, callback);
    }

    private void Listen<T>(string methodName, Action<T> callback) where T : ServerEvent
    {
        ArgumentNullException.ThrowIfNull(_connection);

        _connection.On<string, string>(
            methodName,
            (content, type) =>
            {
                if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(type))
                {
                    return;
                }

                var @event = ServerHelper.ResolveEvent(content, type);
                callback.Invoke((T)@event);
            });
    }

    public string? GetId()
    {
        ArgumentNullException.ThrowIfNull(_connection);

        return _connection.ConnectionId;
    }

    public ValueTask DisposeAsync()
    {
        return _connection?.DisposeAsync() ?? default;
    }
}