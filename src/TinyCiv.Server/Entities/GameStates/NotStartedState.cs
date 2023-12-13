using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Client.Lobby;

namespace TinyCiv.Server.Entities.GameStates;

public class NotStartedState : IGameState
{
    public bool HandleEvent(ClientEvent @event)
    {
        return @event is StartGameClientEvent || 
            @event is JoinLobbyClientEvent || 
            @event is LeaveLobbyClientEvent;
    }
}
