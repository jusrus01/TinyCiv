using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class UnitAddHandler : ClientHandler<AddNewUnitClientEvent>
{
    private readonly IMapService _mapService;

    public UnitAddHandler(IMapService mapService)
    {
        _mapService = mapService;
    }
    
    protected override async Task OnHandleAsync(IClientProxy caller, IClientProxy all, AddNewUnitClientEvent @event)
    {
        // // TODO: consider moving the responsibility of validation to other classes and not handlers
        // // e.g.: make handler execute ONLY when it can be executed
        // if (!_sessionService.IsSessionStarted() || !_sessionService.IsValidPlayer(@event.PlayerId))
        // {
        //     return;
        // }
        //
        // // TODO: add validation if the player can move at all
        // // TODO: add other needed validation
        // _sessionService.PlaceUnit(@event.X, @event.Y);
        // await all
        //     .SendEventAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_sessionService.GetMap()))
        //     .ConfigureAwait(false);

        try
        {
            _mapService.AddUnit(@event.PlayerId, new Position { X = @event.X, Y = @event.Y });
        }
        catch
        {
            return;
        }

        await all
            .SendEventAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
            .ConfigureAwait(false);
    }
}