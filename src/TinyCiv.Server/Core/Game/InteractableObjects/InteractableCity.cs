using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableCity : IInteractableObject
{
    public int Price => 0;
    public int AttackDamage => Constants.Game.Interactable.City.Damage;
    public int AttackRateInMilliseconds => Constants.Game.Interactable.AttackIntervalInMilliseconds;
    public bool IsAbleToCounterAttack => true;
    public bool IsBuilding => true;
    
    public Guid GameObjectReferenceId { get; init; }
    public int Health { get; set; } = Constants.Game.Interactable.City.InitialHealth;
    
    public void DoDamage(IInteractableObject interactable)
    {
        interactable.Health -= AttackDamage;
    }
}