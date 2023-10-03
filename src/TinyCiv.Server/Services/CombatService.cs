using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class CombatService : ICombatService
{
    private const int DealDamageEachMilliseconds = 500;

    private readonly object _combatLocker;
    
    private readonly IMapService _mapService;
    private readonly ILogger<CombatService> _logger;

    public CombatService(IMapService mapService, ILogger<CombatService> logger)
    {
        _mapService = mapService;
        _logger = logger;
        _combatLocker = new object();
    }
    
    public Task InitiateCombatAsync(Guid opponentId, Func<Map, Task> combatNotifier)
    {
        return Task.Run(async () =>
        {
            ServerGameObject? objToAttack;
            ServerGameObject? attacker;
            lock (_combatLocker)
            {
                objToAttack = _mapService.GetUnit(opponentId);
                attacker = _mapService.GetUnit(objToAttack?.OpponentId);
            }
            
            if (!IsAlive(objToAttack))
            {
                _logger.LogWarning("Object that needs to be attacked '{object_to_attack}' by '{attacker_id}' is dead", objToAttack?.Id, attacker?.Id);
                return;
            }

            if (!IsAlive(attacker))
            {
                _logger.LogWarning("Object that needs to attack '{attacker_id}' for '{object_to_attack}' is dead", attacker?.Id, objToAttack?.Id);
                return;
            }
            
            while (IsAttacking(objToAttack) && IsAlive(attacker) && IsAlive(objToAttack))
            {
                lock (_combatLocker)
                {
                    if (
                        objToAttack?.ServerUnitProperties == null ||
                        attacker?.ServerUnitProperties == null)
                    {
                        return;
                    }
                    
                    objToAttack.ServerUnitProperties.Health -= attacker.ServerUnitProperties.AttackDamage;
                    
                    combatNotifier(_mapService.GetMap() ?? throw new InvalidOperationException()).GetAwaiter().GetResult();
                }
                
                await Task.Delay(DealDamageEachMilliseconds);
            }

            if (!IsAlive(objToAttack))
            {
                lock (_combatLocker)
                {
                    if (attacker != null)
                    {
                        attacker.OpponentId = null;
                    }

                    if (objToAttack != null)
                    {
                        objToAttack.OpponentId = null;
                    }
                    
                    _mapService.ReplaceWithEmpty(objToAttack?.Id ?? Guid.Empty);
                    combatNotifier(_mapService.GetMap() ?? throw new InvalidOperationException()).GetAwaiter().GetResult();
                }
            }
        });
    }

    private bool IsAlive(ServerGameObject? objToAttack)
    {
        var isAlive = objToAttack != null && objToAttack.ServerUnitProperties?.Health > 0;
        // _logger.LogInformation("Object '{object_id}' has this state and is alive: '{alive}', obj info: '{@obj}'", objToAttack?.Id, isAlive, objToAttack);
        return isAlive;
    }

    private static bool IsAttacking(ServerGameObject? objToAttack)
    {
        // If object has an opponent, means it is still being attacked
        return objToAttack?.OpponentId != null;
    }
}