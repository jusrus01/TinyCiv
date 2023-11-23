using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Entities.GameStates;

public class NotStartedState : IGameState
{
    public bool HandleEvent(ClientEvent @event)
    {
        return @event is StartGameClientEvent;
    }
}
