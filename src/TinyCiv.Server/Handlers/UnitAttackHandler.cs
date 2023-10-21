using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Dtos.Units;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers;

public class UnitAttackHandler : ClientHandler<AttackUnitClientEvent>
{
    private readonly ISessionService _sessionService;
    private readonly IGameService _gameService;

    public UnitAttackHandler(
        ISessionService sessionService,
        ILogger<IClientHandler> logger,
        IGameService gameService,
        IPublisher publisher)
        :
        base(publisher, logger)
    {
        _sessionService = sessionService;
        _gameService = gameService;
    }

    protected override bool IgnoreWhen(AttackUnitClientEvent @event) =>
        !_sessionService.IsStarted();
    
    protected override Task OnHandleAsync(AttackUnitClientEvent @event)
    {
        var request = new AttackUnitRequest(@event.AttackerId, @event.OpponentId, MapChangeNotifier, InteractableObjectStateChangeNotifier, NewUnitNotifier);
        _gameService.AttackUnit(request);
        return Task.CompletedTask;
        
        Task MapChangeNotifier(Map updatedMap) =>
            NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(updatedMap));

        Task NewUnitNotifier(ServerGameObject gameObject) =>
            NotifyCallerAsync(Constants.Server.SendCreatedUnit, new CreateUnitServerEvent(gameObject));
        
        Task InteractableObjectStateChangeNotifier(IInteractableObject interactableObject) =>
            NotifyAllAsync(Constants.Server.SendInteractableObjectChangesToAll, new InteractableObjectServerEvent(interactableObject.GameObjectReferenceId, interactableObject.Health, interactableObject.AttackDamage));
    }
}