using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Entities.GameStates;

public class OnlyUnitState : IGameState
{
    public bool HandleEvent(ClientEvent @event)
    {
        return @event is not PlaceTownClientEvent || @event is not CreateBuildingClientEvent;
    }
}
