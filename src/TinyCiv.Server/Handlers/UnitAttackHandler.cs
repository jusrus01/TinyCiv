using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers;

public class UnitAttackHandler : ClientHandler<AttackUnitClientEvent>
{
    private readonly ISessionService _sessionService;
    private readonly ICombatService _combatService;

    public UnitAttackHandler(
        ISessionService sessionService,
        ICombatService combatService,
        ILogger<IClientHandler> logger)
        :
        base(logger)
    {
        _sessionService = sessionService;
        _combatService = combatService;
    }

    protected override bool IgnoreWhen(AttackUnitClientEvent @event) =>
        !_sessionService.IsStarted();
    
    protected override Task OnHandleAsync(AttackUnitClientEvent @event)
    {
        return _combatService.InitiateCombatAsync(@event.OpponentId, CombatNotifier);
        
        Task CombatNotifier(Map updatedMap) =>
            NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(updatedMap));
    }
}