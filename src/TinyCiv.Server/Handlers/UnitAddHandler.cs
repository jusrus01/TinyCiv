using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;
using TinyCiv.Server.Dtos.Units;

namespace TinyCiv.Server.Handlers;

public class UnitAddHandler : ClientHandler<CreateUnitClientEvent>
{
    private readonly IGameService _gameService;

    public UnitAddHandler(ILogger<UnitAddHandler> logger, IGameService gameService) : base(logger)
    {
        _gameService = gameService;
    }

    protected override async Task OnHandleAsync(CreateUnitClientEvent @event)
    {
        var position = new ServerPosition { X = @event.X, Y = @event.Y };
        var request = new AddUnitRequest(@event.PlayerId, position, @event.UnitType);
        var response = _gameService.AddUnit(request);

        if (response == null)
        {
            return;
        }
        
        await NotifyCallerAsync(Constants.Server.SendCreatedUnit, new CreateUnitServerEvent(response.Unit))
            .ConfigureAwait(false);

        var interactableEvent = response.Events?.SingleOrDefault(e => e is InteractableObjectServerEvent);
        var resourceEvent = response.Events?.SingleOrDefault(e => e is ResourcesUpdateServerEvent);

        // Start notification tasks
        var interactableNotifyTask = interactableEvent != null
            ? NotifyAllAsync(Constants.Server.SendInteractableObjectChangesToAll, (InteractableObjectServerEvent)interactableEvent)
            : Task.CompletedTask;
        var resourceNotifyTask = resourceEvent != null
            ? NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, (ResourcesUpdateServerEvent)resourceEvent)
            : Task.CompletedTask;

        await Task.WhenAll(interactableNotifyTask, resourceNotifyTask);
        
        // Trigger map update when interactable and resources are updated
        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(response.Map))
            .ConfigureAwait(false);
    }

    protected override bool IgnoreWhen(CreateUnitClientEvent @event)
    {
        return @event.UnitType != GameObjectType.Cavalry &&
            @event.UnitType != GameObjectType.Warrior &&
            @event.UnitType != GameObjectType.Tarran;
    }
}