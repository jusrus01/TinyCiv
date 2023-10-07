using System.Collections.Concurrent;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class CombatService : ICombatService
{
    private readonly IMapService _mapService;
    private readonly IInteractableObjectService _interactableObjectService;

    private readonly ConcurrentDictionary<Guid, Guid> _combatParticipants;
    private readonly object _combatLocker;

    public CombatService(IMapService mapService, IInteractableObjectService interactableObjectService)
    {
        _mapService = mapService;
        _interactableObjectService = interactableObjectService;
        _combatParticipants = new ConcurrentDictionary<Guid, Guid>();
        _combatLocker = new object();
    }
    
    public Task InitiateCombatAsync(
        Guid attackerId,
        Guid opponentId, Func<Map, Task> mapChangeNotifier,
        Func<IInteractableObject, Task> attackStateNotifier)
    {
        if (_combatParticipants.ContainsKey(attackerId))
        {
            return Task.CompletedTask;
        }
        
        var attacker = _mapService.GetUnit(attackerId);
        if (attacker == null)
        {
            return Task.CompletedTask;
        }
        
        var attackerInteractableObject = _interactableObjectService.Get(attackerId);
        if (attackerInteractableObject == null)
        {
            return Task.CompletedTask;
        }
        
        var interactableUnderAttackObject = _interactableObjectService.Get(opponentId);
        if (interactableUnderAttackObject == null)
        {
            return Task.CompletedTask;
        }

        var objectUnderAttack = _mapService.GetUnit(opponentId);
        if (objectUnderAttack == null)
        {
            return Task.CompletedTask;
        }

        _combatParticipants.TryAdd(attackerId, opponentId);

        return Task.Run(async () =>
        {
            var isCombatActive = true;
            while (isCombatActive)
            {
                lock (_combatLocker)
                {
                    var isAttackerAlive = _interactableObjectService.IsAlive(attackerInteractableObject);
                    if (!isAttackerAlive)
                    {
                        _combatParticipants.Remove(attackerId, out _);
                        _interactableObjectService.Remove(attackerId);
                        _mapService.ReplaceWithEmpty(attackerId);
                    }
                    
                    var isObjectUnderAttackAlive = _interactableObjectService.IsAlive(interactableUnderAttackObject);
                    if (!isObjectUnderAttackAlive)
                    {
                        _combatParticipants.Remove(opponentId, out _);
                        _interactableObjectService.Remove(opponentId);
                        _mapService.ReplaceWithEmpty(opponentId);
                    }

                    // TODO: consider not relying on map service,
                    // or at least get "agro" state from map service
                    var isAttacking = attacker.OpponentId != null;
                    if (!isAttacking)
                    {
                        _combatParticipants.Remove(attackerId, out _);
                    }
                    
                    isCombatActive = isAttackerAlive && isObjectUnderAttackAlive && isAttacking;

                    if (!isCombatActive && (!isAttackerAlive || !isObjectUnderAttackAlive))
                    {
                        mapChangeNotifier(_mapService.GetMap() ?? throw new Exception()).GetAwaiter().GetResult();
                    }
                    
                    if (!isCombatActive)
                    {
                        return Task.CompletedTask;
                    }

                    interactableUnderAttackObject.Health -= attackerInteractableObject.AttackDamage;
                    var attackedStateTask = attackStateNotifier(interactableUnderAttackObject);

                    var isAfterAttackAlive = _interactableObjectService.IsAlive(interactableUnderAttackObject);
                    if (!isAfterAttackAlive)
                    {
                        _combatParticipants.Remove(opponentId, out _);
                        _mapService.ReplaceWithEmpty(opponentId);
                        _interactableObjectService.Remove(opponentId);
                    }

                    if (!isAfterAttackAlive || !isAttackerAlive || !isObjectUnderAttackAlive)
                    {
                        attackedStateTask.GetAwaiter().GetResult();
                        mapChangeNotifier(_mapService.GetMap() ?? throw new Exception()).GetAwaiter().GetResult();
                    }
                    else
                    {
                        attackedStateTask.GetAwaiter().GetResult();
                    }
                }

                await Task.Delay(attackerInteractableObject.AttackRateInMilliseconds);
            }

            return Task.CompletedTask;
        });
    }
}