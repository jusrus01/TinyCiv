using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class PlaceCityHandler : ClientHandler<PlaceCityClientEvent>
{
    private readonly IMapService _mapService;

    public PlaceCityHandler(ILogger<IClientHandler> logger, IMapService mapService) : base(logger)
    {
        _mapService = mapService;
    }

    protected override async Task OnHandleAsync(PlaceCityClientEvent @event)
    {
        if (_mapService.IsCityOwner(@event.PlayerId))
        {
            return;
        }

        bool result = _mapService.PlaceCity(@event.PlayerId);

        if (result == false)
        {
            return;
        }

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
            .ConfigureAwait(false);
    }
}
