using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Middleware;

public class StateMiddleware<TEvent> : Core.Middlewares.Middleware<TEvent>
{
    private readonly IGameStateService _gameStateService;

    public StateMiddleware(IGameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    public override bool HandleInternal(TEvent @event)
    {
        var clientEvent = @event as ClientEvent;

        if (clientEvent == null)
        {
            return false;
        }

        return _gameStateService.GetState().HandleEvent(clientEvent);
    }
}
