namespace TinyCiv.Server.Core.Game.InteractableObjects;

public interface IInteractableObject : IInteractableInfo
{
    Guid GameObjectReferenceId { get; }
    int Health { get; set; }
    int AttackDamage { get; }
    int AttackRateInMilliseconds { get; }
    bool IsAbleToCounterAttack { get; }
    bool IsBuilding { get; }

    // Some interactables have special effects, depending
    // on what is being attacked
    void DoDamage(IInteractableObject interactable);
}