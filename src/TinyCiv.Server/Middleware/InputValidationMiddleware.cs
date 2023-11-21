using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Middleware;

public class InputValidationMiddleware<TEvent> : Core.Middlewares.Middleware<TEvent>
{
    public override bool HandleInternal(TEvent @event)
    {
        if (@event == null)
        {
            return false;
        }

        bool isInputValid = true;

        if (@event is AttackUnitClientEvent attackEvent)
        {
            if (attackEvent.PlayerId == null || attackEvent.OpponentId == null || attackEvent.AttackerId == null)
            {
                isInputValid = false;
            }
        }

        if (@event is MoveUnitClientEvent moveUnitEvent)
        {
            if (moveUnitEvent.PlayerId == null || moveUnitEvent.UnitId == null)
            {
                isInputValid = false;
            }
        }

        return isInputValid;
    }
}
