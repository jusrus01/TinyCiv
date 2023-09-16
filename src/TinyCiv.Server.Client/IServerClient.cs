using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Client;

public interface IServerClient
{
    Task SendAsync<T>(T @event, CancellationToken token = default) where T : ClientEvent;

    void ListenForPlayerIdAssignment(Action<JoinLobbyServerEvent> callback);
    void ListenForGameStart(Action<GameStartServerEvent> callback);
    void ListenForMapChange(Action<MapChangeServerEvent> callback);
}