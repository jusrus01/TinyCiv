using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Client;

public interface IServerClient
{
    Task SendAsync<T>(T @event, CancellationToken token = default) where T : ClientEvent;

    void ListenForUnitStatusUpdate(Action<UnitStatusUpdateServerEvent> callback);
    void ListenForNewPlayerCreation(Action<JoinLobbyServerEvent> callback);
    void ListenForNewUnitCreation(Action<CreateUnitServerEvent> callback);
    void ListenForGameStart(Action<GameStartServerEvent> callback);
    void ListenForMapChange(Action<MapChangeServerEvent> callback);
    void ListenForResourcesUpdate(Action<ResourcesUpdateServerEvent> callback);
    void ListenForLobbyState(Action<LobbyStateServerEvent> callback);
    void ListenForInteractableObjectChanges(Action<InteractableObjectServerEvent> callback);
    void ListenForVictoryEvent(Action<VictoryServerEvent> callback);
    void ListenForDefeatEvent(Action<DefeatServerEvent> callback);
}