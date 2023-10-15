using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableTarran : IInteractableObject
{
    public int AttackDamage => Constants.Game.Interactable.Tarran.Damage;
    public int AttackRateInMilliseconds => Constants.Game.Interactable.AttackIntervalInMilliseconds;
    public bool IsAbleToCounterAttack => false;
    public bool IsBuilding => false;
    public int Price => Constants.Game.Interactable.Tarran.Price;

    public int Health { get; set; } = Constants.Game.Interactable.Tarran.InitialHealth;
    public Guid GameObjectReferenceId { get; init; }

    public void DoDamage(IInteractableObject interactable)
    {
        if (interactable.IsBuilding)
        {
            interactable.Health -= AttackDamage * Constants.Game.Interactable.Tarran.BuildingAttackMultiplier;
        }
        else
        {
            interactable.Health -= AttackDamage;
        }
    }
}