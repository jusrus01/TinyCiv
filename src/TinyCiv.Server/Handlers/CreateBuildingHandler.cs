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
    public CreateBuildingHandler(ILogger<IClientHandler> logger, IGameService gameService, IPublisher publisher) : base(publisher, gameService, logger)
    {
    }

    protected override async Task OnHandleAsync(CreateBuildingClientEvent @event)
    {
        async void ResourceUpdateCallback(Resources resources)
        {
            await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(resources))
                .ConfigureAwait(false);
        }

        var request = new CreateBuildingRequest(@event.PlayerId, @event.BuildingType, @event.Position, ResourceUpdateCallback);
        var response = GameService.CreateBuilding(request);

        if (response == null) return;

        await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(response.Resources))
            .ConfigureAwait(false);

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(response.Map))
            .ConfigureAwait(false);
    }
}
