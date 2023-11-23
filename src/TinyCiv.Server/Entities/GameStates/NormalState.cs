using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Entities.GameStates;

public class NormalState : IGameState
{
    public bool HandleEvent(ClientEvent @event)
    {
        return true;
    }
}
