namespace TinyCiv.Server.Core.Game.InteractableObjects;

public interface IInteractableObject : IInteractableInfo, IPrototype<IInteractableObject>
{
    Guid GameObjectReferenceId { get; set; }
    int InitialHealth { get; }
    int Health { get; set; }
    int AttackDamage { get; }
    int AttackRateInMilliseconds { get; }
    bool IsAbleToCounterAttack { get; }
    bool IsBuilding { get; }
    int? SpawnClonesBeforeDeath { get; set; }
    
    // Some interactables have special effects, depending
    // on what is being attacked
    void DoDamage(IInteractableObject interactable);
}