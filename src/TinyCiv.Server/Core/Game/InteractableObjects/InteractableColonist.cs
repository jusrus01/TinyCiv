using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableColonist : IInteractableObject
{
    public int AttackDamage => 0; // Constants.Game.Interactable.Colonist.Damage;
    public int AttackRateInMilliseconds => 0; // Constants.Game.Interactable.AttackIntervalInMilliseconds;
    public bool IsAbleToCounterAttack => false;
    public bool IsBuilding => false;

    public int Health { get; set; } = 0; // Constants.Game.Interactable.Colonist.InitialHealth;
    public Guid GameObjectReferenceId { get; init; }

    public void DoDamage(IInteractableObject interactable)
    {
        interactable.Health -= AttackDamage;
    }
}