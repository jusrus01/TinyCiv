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
    private readonly IGameService _gameService;

    private readonly ConcurrentDictionary<Guid, Guid> _combatParticipants;
    private readonly object _combatLocker;

    public CombatService(IMapService mapService, IInteractableObjectService interactableObjectService, IGameService gameService)
    {
        _mapService = mapService;
        _interactableObjectService = interactableObjectService;
        _gameService = gameService;
        
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
                        _combatParticipants.Remove(attackerId, out _);
                        _interactableObjectService.Remove(attackerId);
                        _mapService.ReplaceWithEmpty(attackerId);
                    }

                    var isObjectUnderAttackAlive = objectUnderAttackController.IsAlive();
                    if (!isObjectUnderAttackAlive)
                    {
                        _combatParticipants.Remove(opponentId, out _);
                        _interactableObjectService.Remove(opponentId);
                        _mapService.ReplaceWithEmpty(opponentId);
                    }

                    var isAttacking = objectUnderAttackController.IsUnderAttack(attacker);
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
                    
                    attackerController.Attack(interactableUnderAttackObject);


                    var isAfterAttackAlive = objectUnderAttackController.IsAlive();
                    if (!isAfterAttackAlive)
                    {
                        _combatParticipants.Remove(opponentId, out _);
                        _mapService.ReplaceWithEmpty(opponentId);
                        _interactableObjectService.Remove(opponentId);
                    }

                    if (!isAfterAttackAlive || !isAttackerAlive || !isObjectUnderAttackAlive)
                    {
                        var clones = _interactableObjectService.FlushClones()?.ToList();

                        // if clones exists, then let the TransformToGameObjectsAsync handle map updates 
                        if (clones?.Any() ?? false)
                        {
                            _gameService
                                .TransformToGameObjectsAsync(clones, mapChangeNotifier, attackStateNotifier, newUnitNotifier)
                                .GetAwaiter()
                                .GetResult();
                        }
                        else
                        {
                            mapChangeNotifier(_mapService.GetMap() ?? throw new Exception()).GetAwaiter().GetResult();
                        }
                    }
                }
            }

            return Task.CompletedTask;
        });
    }
}