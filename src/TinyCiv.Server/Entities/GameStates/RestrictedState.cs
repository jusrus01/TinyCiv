using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Entities.GameStates;

public class RestrictedState : IGameState
{
    public bool HandleEvent(ClientEvent @event)
    {
        return @event is not AttackUnitClientEvent ||
            @event is not CreateUnitClientEvent ||
            @event is not MoveUnitClientEvent ||
            @event is not CreateBuildingClientEvent ||
            @event is not PlaceTownClientEvent;
    }
}
