namespace TinyCiv.Server.Core.Game.InteractableObjects;

public interface IInteractableObject
{
    public Guid GameObjectReferenceId { get; }
    public int Health { get; set; }
    public int AttackDamage { get; }
    public int AttackRateInMilliseconds { get; }
    public bool IsAbleToCounterAttack { get; }
    public bool IsBuilding { get; }

    // Some interactables have special effects, depending
    // on what is being attacked
    void DoDamage(IInteractableObject interactable);
}