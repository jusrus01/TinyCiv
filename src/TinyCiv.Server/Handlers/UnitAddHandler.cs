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
        ServerGameObject unit;

        try
        {
            unit = _mapService.AddUnit(@event.PlayerId, new ServerPosition { X = @event.X, Y = @event.Y });
        }
        catch
        {
            return;
        }

        await caller
            .SendEventAsync(Constants.Server.SendCreatedUnit, new AddNewUnitServerEvent(unit))
            .ConfigureAwait(false);

        await all
            .SendEventAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
            .ConfigureAwait(false);
    }
}