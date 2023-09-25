using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class UnitAddHandler : ClientHandler<CreateUnitClientEvent>
{
    private readonly IMapService _mapService;

    public UnitAddHandler(IMapService mapService, ILogger<UnitAddHandler> logger) : base(logger)
    {
        _mapService = mapService;
    }
    
    protected override async Task OnHandleAsync(IClientProxy caller, IClientProxy all, CreateUnitClientEvent @event)
    {
        var unit = _mapService.CreateUnit(@event.PlayerId, new ServerPosition { X = @event.X, Y = @event.Y });

        if (unit == null)
        {
            return;
        }

        await caller
            .SendEventAsync(Constants.Server.SendCreatedUnit, new CreateUnitServerEvent(unit))
            .ConfigureAwait(false);

        await all
            .SendEventAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
            .ConfigureAwait(false);
    }
}