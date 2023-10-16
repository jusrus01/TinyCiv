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

        if (response == null) return;
        
        await NotifyCallerAsync(Constants.Server.SendCreatedUnit, new CreateUnitServerEvent(response.Unit))
            .ConfigureAwait(false);

        // Do not notify about map change before created unit receives properties such as Health, AttackDamage
        if (response.InteractableObjectEvent != null)
        {
            await NotifyAllAsync(Constants.Server.SendInteractableObjectChangesToAll, response.InteractableObjectEvent);
        }
        
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