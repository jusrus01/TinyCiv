using TinyCiv.Server.Core.Game.InteractableObjects;
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
    private readonly Func<IInteractableObject, Task> _attackStateNotifier;
    private readonly IInteractableObject? _initiator;

    public InteractableController(
        IInteractableObject initiator,
        Func<IInteractableObject, Task> attackStateNotifier)
    {
        _initiator = initiator;
        _attackStateNotifier = attackStateNotifier;
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

        _initiator.DoDamage(interactable);

        Task? counterAttackTask = null;
        if (interactable.IsAbleToCounterAttack)
        {
            interactable.DoDamage(_initiator);
            
            counterAttackTask = _attackStateNotifier(_initiator);
        }

        Task.WhenAll(_attackStateNotifier(interactable), counterAttackTask ?? Task.CompletedTask).GetAwaiter().GetResult();
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