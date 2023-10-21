using System.Collections.Concurrent;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Game.InteractableObjects;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class CombatService : ICombatService
{
    private readonly IMapService _mapService;
    private readonly IInteractableObjectService _interactableObjectService;

    private readonly ConcurrentDictionary<Guid, Guid> _combatParticipants;
    private readonly object _combatLocker;
    private readonly ILogger<CombatService> _logger;

    public CombatService(ILogger<CombatService> logger, IMapService mapService, IInteractableObjectService interactableObjectService)
    {
        _mapService = mapService;
        _interactableObjectService = interactableObjectService;
        _logger = logger;
        
        _combatParticipants = new ConcurrentDictionary<Guid, Guid>();
        _combatLocker = new object();
    }
    
    public Task InitiateCombatAsync(
        Guid attackerId,
        Guid opponentId,
        Func<Map, Task> mapChangeNotifier,
        Func<IInteractableObject, Task> attackStateNotifier,
        Func<ServerGameObject, Task> newUnitNotifier)
    {
        if (_combatParticipants.ContainsKey(attackerId))
        {
            return Task.CompletedTask;
        }
        
        var attacker = _mapService.GetUnit(attackerId);
        if (attacker == null)
        {
            _logger.LogWarning("Attacker was not found in map, skipping combat '{attacker_id}'", attackerId);
            return Task.CompletedTask;
        }
        
        var attackerInteractableObject = _interactableObjectService.Get(attackerId);
        if (attackerInteractableObject == null)
        {
            _logger.LogWarning("Attacker was not found in map, skipping combat '{attacker_id}'", attackerId);
            return Task.CompletedTask;
        }
        
        var interactableUnderAttackObject = _interactableObjectService.Get(opponentId);
        if (interactableUnderAttackObject == null)
        {
            _logger.LogWarning("Object under attack was not found in map, skipping combat '{object_id}'", opponentId);
            return Task.CompletedTask;
        }

        _combatParticipants.TryAdd(attackerId, opponentId);

        return Task.Run(async () =>
        {
            IInteractableController attackerController = new InteractableController(attackerInteractableObject, _interactableObjectService, attackStateNotifier);
            IInteractableController objectUnderAttackController = new InteractableController(interactableUnderAttackObject, _interactableObjectService, attackStateNotifier);
            
            var isCombatActive = true;
            while (isCombatActive)
            {
                await attackerController.WaitAsync();
                
                lock (_combatLocker)
                {
                    var isAttackerAlive = attackerController.IsAlive();
                    if (!isAttackerAlive)
                    {
                        _logger.LogInformation("Attacker '{attacker_id}' is dead", attackerId);
                        _combatParticipants.Remove(attackerId, out _);
                        _interactableObjectService.Remove(attackerId);
                        _mapService.ReplaceWithEmpty(attackerId);
                    }

                    var isObjectUnderAttackAlive = objectUnderAttackController.IsAlive();
                    if (!isObjectUnderAttackAlive)
                    {
                        _logger.LogInformation("Object under attack '{object_id}' is dead", opponentId);
                        _combatParticipants.Remove(opponentId, out _);
                        _interactableObjectService.Remove(opponentId);
                        _mapService.ReplaceWithEmpty(opponentId);
                    }

                    var isAttacking = objectUnderAttackController.IsUnderAttack(attacker);
                    if (!isAttacking)
                    {
                        _logger.LogInformation("Attacker '{attacker_id}' stopped attacking object under attack '{object_id}'", attackerId, opponentId);
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
                    
                    attackerController.Attack(interactableUnderAttackObject);
                    
                    var isAfterAttackAlive = objectUnderAttackController.IsAlive();
                    if (!isAfterAttackAlive)
                    {
                        _combatParticipants.Remove(opponentId, out _);
                        _mapService.ReplaceWithEmpty(opponentId);
                        _interactableObjectService.Remove(opponentId);
                    }

                    var mapUpdateRequired = true;
                    var clones = _interactableObjectService.FlushClones()?.ToList();
                    // if clones exists, then let the TransformToGameObjectsAsync handle map updates 
                    if (clones?.Any() ?? false)
                    {
                        _interactableObjectService
                            .TransformClonesToGameObjectsAsync(clones, mapChangeNotifier, attackStateNotifier, newUnitNotifier)
                            .GetAwaiter()
                            .GetResult();
                        mapUpdateRequired = false;
                    }
                    
                    if ((!isAfterAttackAlive || !isAttackerAlive || !isObjectUnderAttackAlive) && mapUpdateRequired)
                    {
                        mapChangeNotifier(_mapService.GetMap() ?? throw new Exception()).GetAwaiter().GetResult();
                    }
                }
            }

            return Task.CompletedTask;
        });
    }
}