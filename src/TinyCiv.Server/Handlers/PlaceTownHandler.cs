using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class PlaceTownHandler : ClientHandler<PlaceTownClientEvent>
{
    private readonly IMapService _mapService;

    public PlaceTownHandler(ILogger<IClientHandler> logger, IMapService mapService) : base(logger)
    {
        _mapService = mapService;
    }

    protected override async Task OnHandleAsync(PlaceTownClientEvent @event)
    {
        if (_mapService.IsTownOwner(@event.PlayerId))
        {
            return;
        }

        bool result = _mapService.PlaceTown(@event.PlayerId);

        if (result == false)
        {
            return;
        }

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
            .ConfigureAwait(false);
    }
}
