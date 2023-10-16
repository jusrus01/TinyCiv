using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Dtos.Buildings;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers;

public class CreateBuildingHandler : ClientHandler<CreateBuildingClientEvent>
{
    private readonly IMapService _mapService;
    private readonly IResourceService _resourceService;
    private readonly IGameService _gameService;

    public CreateBuildingHandler(ILogger<IClientHandler> logger, IMapService mapService, IResourceService resourceService, IGameService gameService) : base(logger)
    {
        _mapService = mapService;
        _resourceService = resourceService;
        _gameService = gameService;
    }

    protected override async Task OnHandleAsync(CreateBuildingClientEvent @event)
    {
        async void resourceUpdateCallback(Resources resources)
        {
            await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(resources))
                .ConfigureAwait(false);
        }

        var request = new CreateBuildingRequest(@event.PlayerId, @event.BuildingType, @event.Position, resourceUpdateCallback);
        var response = _gameService.CreateBuilding(request);

        if (response == null) return;

        await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(response.Resources))
            .ConfigureAwait(false);

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(response.Map))
            .ConfigureAwait(false);
    }
}
