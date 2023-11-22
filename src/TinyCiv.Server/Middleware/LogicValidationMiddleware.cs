using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Middleware;

public class LogicValidationMiddleware<TEvent> : Core.Middlewares.Middleware<TEvent>
{
    private readonly IMapService _mapService;

    public LogicValidationMiddleware(IMapService mapService)
    {
        _mapService = mapService;
    }

    public override bool HandleInternal(TEvent @event)
    {
        if (@event == null)
        {
            return false;
        }

        bool isEventValid = true;

        if (@event is AttackUnitClientEvent attackEvent)
        {
            var unit = _mapService.GetUnit(attackEvent.OpponentId);

            if (unit == null)
            {
                isEventValid = false;
            }
        }

        if (@event is CreateBuildingClientEvent createBuildingEvent)
        {
            isEventValid = IsPositionValid(createBuildingEvent.Position.X, createBuildingEvent.Position.Y);
        }

        if (@event is CreateUnitClientEvent createUnitEvent)
        {
            isEventValid = IsPositionValid(createUnitEvent.X, createUnitEvent.Y);
        }

        if (@event is MoveUnitClientEvent moveUnitEvent)
        {
            isEventValid = IsPositionValid(moveUnitEvent.X, moveUnitEvent.Y);
        }

        return isEventValid;
    }

    private bool IsPositionValid(int x, int y)
    {
        if (x < 0 || x >= Constants.Game.WidthSquareCount ||
            y < 0 || y >= Constants.Game.HeightSquareCount)
        {
            return false;
        }

        return true;
    }
}
