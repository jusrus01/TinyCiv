using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.InteractableObjects;

/// <summary>
/// Pattern: Bridge (2 of 2)
/// Reasoning:
/// Each interactable object can contain specific way to attack and deal different amount
/// of damage based on some conditions e.g. <see cref="InteractableTarran"/> does more damage to building,
/// than any other type of interactable.
/// </summary>
public class InteractableController : IInteractableController
{
    private const int ConsiderSpawningClonesAtHealthPercentage = 30;
    
    private readonly Func<IInteractableObject, Task> _attackStateNotifier;
    private readonly IInteractableObject? _initiator;
    private readonly IInteractableObjectService _interactableObjectService;

    public InteractableController(
        IInteractableObject initiator,
        IInteractableObjectService interactableObjectService,
        Func<IInteractableObject, Task> attackStateNotifier)
    {
        _initiator = initiator;
        _attackStateNotifier = attackStateNotifier;
        _interactableObjectService = interactableObjectService;
    }
    
    public void Attack(IInteractableObject? interactable)
    {
        if (_initiator == null)
        {
            return;
        }
        
        if (interactable == null)
        {
            return;
        }

        // Building cannot initiate attack,
        // however, can counter attack when they are being attacked
        if (_initiator.IsBuilding)
        {
            return;
        }

        _initiator.DoDamage(interactable);
        if (IsCloningApplicable(interactable))
        {
            CreateClones(interactable);
        }

        Task? counterAttackTask = null;
        if (interactable.IsAbleToCounterAttack)
        {
            interactable.DoDamage(_initiator);
            
            counterAttackTask = _attackStateNotifier(_initiator);
        }

        Task.WhenAll(_attackStateNotifier(interactable), counterAttackTask ?? Task.CompletedTask).GetAwaiter().GetResult();
    }

    private void CreateClones(IInteractableObject interactable)
    {
        ArgumentNullException.ThrowIfNull(interactable.SpawnClonesBeforeDeath);
        
        for (var i = 0; i < interactable.SpawnClonesBeforeDeath; i++)
        {
            var clone = interactable.Clone();
            _interactableObjectService.RegisterClone(clone);
        }
        
        interactable.SpawnClonesBeforeDeath = null;
    }

    private static bool IsCloningApplicable(IInteractableObject interactable)
    {
        if (interactable.SpawnClonesBeforeDeath is <= 0)
        {
            return false;
        }

        var spawnAtHealth = (int)(interactable.InitialHealth * ConsiderSpawningClonesAtHealthPercentage * 0.01);
        return spawnAtHealth <= interactable.Health && interactable.Health > 0;
    }

    public bool IsAlive()
    {
        return _initiator != null && _initiator.Health > 0;
    }

    public bool IsUnderAttack(ServerGameObject? obj)
    {
        var id = _initiator?.GameObjectReferenceId;
        if (!id.HasValue)
        {
            return false;
        }

        return id == obj?.OpponentId;
    }

    public Task WaitAsync()
    {
        var waitInterval = _initiator?.AttackRateInMilliseconds;
        return !waitInterval.HasValue ? Task.CompletedTask : Task.Delay(waitInterval.Value);
    }
}