using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers.Client;

public class UnitAddHandler : ClientHandler<AddNewUnitClientEvent>
{
    private readonly ISessionService _sessionService;

    public UnitAddHandler(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    protected override async Task OnHandleAsync(IClientProxy caller, IClientProxy all, AddNewUnitClientEvent @event)
    {
        // TODO: consider moving the responsibility of validation to other classes and not handlers
        // e.g.: make handler execute ONLY when it can be executed
        if (!_sessionService.IsSessionStarted() || !_sessionService.IsValidPlayer(@event.PlayerId))
        {
            return;
        }
        
        // TODO: add validation if the player can move at all
        // TODO: add other needed validation
        _sessionService.PlaceUnit(@event.X, @event.Y);
        await all
            .SendEventAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_sessionService.GetMap()))
            .ConfigureAwait(false);
    }
}