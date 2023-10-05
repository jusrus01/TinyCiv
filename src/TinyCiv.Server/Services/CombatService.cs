using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Hubs;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class CombatService : ICombatService, IDisposable
{
    // If this becomes too fast, then we can run into an issue,
    // where the client is unable to invoke attacking for other stuff,
    // so better to keep this longer and increase attack damage
    private const int DealDamageEachMilliseconds = 2000;

    private readonly object _combatLocker;

    private List<string> _opponents;

    private readonly IMapService _mapService;
    private readonly ILogger<CombatService> _logger;

    private readonly IHubContext<ServerHub> _hub;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Thread _combatLogicThread;

    public CombatService(
        IMapService mapService,
        ILogger<CombatService> logger,
        IHubContext<ServerHub> hub)
    {
        _mapService = mapService;
        _logger = logger;
        
        _opponents = new List<string>();
        _combatLocker = new object();

        _cancellationTokenSource = new CancellationTokenSource();

        _combatLogicThread = new Thread(() => RunCombatCycle(_cancellationTokenSource.Token));
        _combatLogicThread.Start();
    }

    private void RunCombatCycle(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {

            
            Thread.Sleep(DealDamageEachMilliseconds);
        }
    }
    
    /// <summary>
    /// Assume that both opponents will send <see cref="opponentId"/>
    /// </summary>>
    public Task InitiateCombatAsync(Guid opponentId, Func<Map, Task> combatNotifier)
    {
        ServerGameObject? objToAttack;
        ServerGameObject? attacker;
        
        lock (_combatLocker)
        {
            if (_opponents.Contains(opponentId.ToString()))
            {
                _logger.LogWarning("Opponent id '{op}' is already in combat", opponentId);
                return Task.CompletedTask;
            }
            
            objToAttack = _mapService.GetUnit(opponentId);
            attacker = _mapService.GetUnit(objToAttack?.OpponentId);

            // If any of them are already dead, then skip
            if (!IsAlive(objToAttack) || !IsAlive(attacker))
            {
                return Task.CompletedTask;
            }
            
            _opponents.Add(opponentId.ToString());
        }

        return Task.Run(async () =>
        {
            bool isCombatActive = false;
            lock (_combatLocker)
            {
                // If any of them are already dead, then skip
                if (!IsAlive(objToAttack) || !IsAlive(attacker))
                {
                    return Task.CompletedTask;
                }
                
                isCombatActive = true;
            }

            throw new NotImplementedException();


            //
            // var objToAttack = _mapService.GetUnit(opponentId);
            // var attacker = _mapService.GetUnit(objToAttack?.OpponentId);
            //
            // while (isCombatActive)
            // {
            //     lock (_combatLocker)
            //     {
            //         isCombatActive = IsAttacking(objToAttack) && IsAlive(attacker) && IsAlive(objToAttack);
            //     }
            //
            //     if (!isCombatActive)
            //     {
            //         break;
            //     }
            //
            //     lock (_combatLocker)
            //     {
            //         if (objToAttack != null && attacker != null)
            //         {
            //             ArgumentNullException.ThrowIfNull(objToAttack.ServerUnitProperties);
            //             ArgumentNullException.ThrowIfNull(attacker.ServerUnitProperties);
            //             
            //             objToAttack.ServerUnitProperties.Health -= attacker.ServerUnitProperties.AttackDamage;
            //
            //             if (!IsAlive(objToAttack) || !IsAlive(attacker))
            //             {
            //                 break;
            //             }
            //
            //             combatNotifier(_mapService.GetMap() ?? throw new InvalidOperationException()).GetAwaiter().GetResult();
            //         }
            //     }
            //     
            //     if (IsAlive(objToAttack))
            //     {
            //         await Task.Delay(DealDamageEachMilliseconds);
            //     }
            // }
            //
            // bool invokeMapChange = false;
            // lock (_combatLocker)
            // {
            //     
            //     if (!IsAlive(objToAttack))
            //     {
            //         _opponents.Remove(opponentId.ToString());
            //         _mapService.ReplaceWithEmpty(opponentId);
            //
            //         if (IsAlive(attacker))
            //         {
            //             ArgumentNullException.ThrowIfNull(attacker);
            //             attacker.OpponentId = null;
            //         }
            //         
            //         invokeMapChange = true;
            //     }
            //     
            //     if (!IsAlive(attacker))
            //     {
            //         _mapService.ReplaceWithEmpty(objToAttack?.OpponentId ?? Guid.Empty);
            //
            //         if (IsAlive(objToAttack))
            //         {
            //             ArgumentNullException.ThrowIfNull(objToAttack);
            //             objToAttack.OpponentId = null;
            //         }
            //         
            //         invokeMapChange = true;
            //     }
            // }
            //
            // if (invokeMapChange)
            // {
            //     combatNotifier(_mapService.GetMap() ?? throw new InvalidOperationException()).GetAwaiter().GetResult();
            // }
        });
    }
    
    // public Task InitiateCombatAsync(Guid opponentId, Func<Map, Task> combatNotifier)
    // {
    //     lock (_combatLocker)
    //     {
    //         _logger.LogInformation("Opponents in combat {ops}", _opponents);
    //         
    //         if (_opponents.Contains(opponentId.ToString()))
    //         {
    //             _logger.LogWarning("Opponent id '{id}' already in combat", opponentId);
    //             return Task.CompletedTask;
    //         }
    //     
    //         _opponents.Add(opponentId.ToString());
    //     }
    //     
    //     // Also, could be that we are waiting for something in the thread pool :)
    //     return Task.Run(async () =>
    //     {
    //         ServerGameObject? objToAttack;
    //         ServerGameObject? attacker;
    //         lock (_combatLocker)
    //         {
    //             objToAttack = _mapService.GetUnit(opponentId);
    //             attacker = _mapService.GetUnit(objToAttack?.OpponentId);
    //         }
    //         
    //         if (!IsAlive(objToAttack))
    //         {
    //             lock (_combatLocker)
    //             {
    //                 _opponents.Remove(attacker?.OpponentId.ToString() ?? string.Empty);
    //             }
    //
    //             _logger.LogWarning("Object that needs to be attacked '{object_to_attack}' by '{attacker_id}' is dead", objToAttack?.Id, attacker?.Id);
    //             return;
    //         }
    //
    //         if (!IsAlive(attacker))
    //         {
    //             lock (_combatLocker)
    //             {
    //                 _opponents.Remove(objToAttack?.OpponentId.ToString() ?? string.Empty);
    //             }
    //             
    //             _logger.LogWarning("Object that needs to attack '{attacker_id}' for '{object_to_attack}' is dead", attacker?.Id, objToAttack?.Id);
    //             return;
    //         }
    //         
    //         while (IsAttacking(objToAttack) && IsAlive(attacker) && IsAlive(objToAttack))
    //         {
    //             lock (_combatLocker)
    //             {
    //                 if (
    //                     objToAttack?.ServerUnitProperties == null ||
    //                     attacker?.ServerUnitProperties == null)
    //                 {
    //                     return;
    //                 }
    //                 
    //                 objToAttack.ServerUnitProperties.Health -= attacker.ServerUnitProperties.AttackDamage;
    //                 _logger.LogInformation("under attack: '{@under_attack}', attacker: '{@attacker}'", objToAttack, attacker);
    //                 
    //                 combatNotifier(_mapService.GetMap() ?? throw new InvalidOperationException()).GetAwaiter().GetResult();
    //             }
    //             
    //             await Task.Delay(DealDamageEachMilliseconds);
    //         }
    //
    //         if (!IsAlive(objToAttack))
    //         {
    //             lock (_combatLocker)
    //             {
    //                 if (attacker != null)
    //                 {
    //                     lock (_combatLocker)
    //                     {
    //                         _opponents.Remove(attacker.OpponentId.ToString() ?? string.Empty);
    //                     }
    //
    //                     attacker.OpponentId = null;
    //                 }
    //
    //                 if (objToAttack != null)
    //                 {
    //                     lock (_combatLocker)
    //                     {
    //                         _opponents.Remove(objToAttack.OpponentId.ToString() ?? string.Empty);
    //                     }
    //                     
    //                     objToAttack.OpponentId = null;
    //                 }
    //                 
    //                 _mapService.ReplaceWithEmpty(objToAttack?.Id ?? Guid.Empty);
    //                 combatNotifier(_mapService.GetMap() ?? throw new InvalidOperationException()).GetAwaiter().GetResult();
    //             }
    //         }
    //     });
    // }

    private static bool IsAlive(ServerGameObject? objToAttack)
    {
        return objToAttack != null && objToAttack.ServerUnitProperties?.Health > 0;
    }

    private static bool IsAttacking(ServerGameObject? objToAttack)
    {
        // If object has an opponent, means it is still being attacked and is attacking something
        return objToAttack?.OpponentId != null;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}