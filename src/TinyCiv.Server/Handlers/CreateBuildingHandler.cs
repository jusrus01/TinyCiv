using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;

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

    protected override Task OnHandleAsync(CreateBuildingClientEvent @event)
    {
        var buildingTile = _mapService.CreateBuilding(@event.PlayerId, @event.Position);

        if (buildingTile == null)
        {
            return Task.CompletedTask;
        }

        var building = BuildingsMapper.Buildings[@event.BuildingType];

        _resourceService.AddBuilding(@event.PlayerId, building);

        return Task.CompletedTask;
    }
}
