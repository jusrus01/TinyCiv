using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Dtos.Buildings;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers;

public class CreateBuildingHandler : ClientHandler<CreateBuildingClientEvent>
{
    private readonly IConnectionIdAccessor _accessor;

    public CreateBuildingHandler(ILogger<IClientHandler> logger, IConnectionIdAccessor accessor, IGameService gameService, IPublisher publisher) : base(publisher, gameService, logger)
    {
        _accessor = accessor;
    }

    protected override async Task OnHandleAsync(CreateBuildingClientEvent @event)
    {
        async void ResourceUpdateCallback(Resources resources)
        {
            await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(resources, _accessor.ConnectionId))
                .ConfigureAwait(false);
        }

        var request = new CreateBuildingRequest(@event.PlayerId, @event.BuildingType, @event.Position, ResourceUpdateCallback);
        var response = GameService.CreateBuilding(request);

        if (response == null) return;

        await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(response.Resources, _accessor.ConnectionId))
            .ConfigureAwait(false);

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(response.Map, _accessor.ConnectionId))
            .ConfigureAwait(false);
    }
}
