using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Middleware;

public class AuthorizationMiddleware<TEvent> : Core.Middlewares.Middleware<TEvent>
{
    private readonly IMapService _mapService;

    public AuthorizationMiddleware(IMapService mapService)
    {
        _mapService = mapService;
    }

    public override bool HandleInternal(TEvent @event)
    {
        if (@event == null)
        {
            return false;
        }

        bool isAuthorized = true;

        if (@event is MoveUnitClientEvent moveEvent)
        {
            isAuthorized = IsAuthorized(moveEvent.PlayerId.Value, moveEvent.UnitId.Value);
        }

        if (@event is AttackUnitClientEvent attackEvent)
        {
            isAuthorized = IsAuthorized(attackEvent.PlayerId.Value, attackEvent.AttackerId.Value);
        }

        return isAuthorized;
    }

    private bool IsAuthorized(Guid PlayerId, Guid UnitId)
    {
        var unit = _mapService.GetUnit(UnitId);

        if (unit == null)
        {
            return false;
        }

        if (unit.OwnerPlayerId != PlayerId)
        {
            return false;
        }

        return true;
    }
}
