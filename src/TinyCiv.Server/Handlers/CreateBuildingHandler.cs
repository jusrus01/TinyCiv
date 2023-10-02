using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers;

public class CreateBuildingHandler : ClientHandler<CreateBuildingClientEvent>
{
    private readonly IMapService _mapService;
    private readonly IResourceService _resourceService;

    public CreateBuildingHandler(ILogger<IClientHandler> logger, IMapService mapService, IResourceService resourceService) : base(logger)
    {
        _mapService = mapService;
        _resourceService = resourceService;
    }

    protected override async Task OnHandleAsync(CreateBuildingClientEvent @event)
    {
        async void resourceUpdateCallback(Resources resources)
        {
            await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(resources))
                .ConfigureAwait(false);
        }

        bool buildingExist = BuildingsMapper.Buildings.TryGetValue(@event.BuildingType, out var building);

        if (buildingExist == false)
        {
            return;
        }

        bool canAfford = _resourceService.GetResources(@event.PlayerId).Industry >= building!.Price;

        if (canAfford == false)
        {
            return;
        }

        _resourceService.AddResources(@event.PlayerId, ResourceType.Industry, -building.Price);
        var playerResources = _resourceService.GetResources(@event.PlayerId);

        await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(playerResources))
            .ConfigureAwait(false);

        var buildingTile = _mapService.CreateBuilding(@event.PlayerId, @event.Position, building!);

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
            .ConfigureAwait(false);

        if (buildingTile == null)
        {
            return;
        }

        _resourceService.AddBuilding(@event.PlayerId, building!, resourceUpdateCallback);

        return;
    }
}
