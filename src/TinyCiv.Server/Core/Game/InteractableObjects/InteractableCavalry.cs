using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableCavalry : IInteractableObject
{
    public int AttackDamage => Constants.Game.Interactable.Cavalry.Damage;
    public int AttackRateInMilliseconds => Constants.Game.Interactable.AttackIntervalInMilliseconds;
    public bool IsAbleToCounterAttack => false;
    public bool IsBuilding => false;
    public int Price => Constants.Game.Interactable.Cavalry.Price;

    public int Health { get; set; } = Constants.Game.Interactable.Cavalry.InitialHealth;
    public Guid GameObjectReferenceId { get; init; }
    
    public void DoDamage(IInteractableObject interactable)
    {
        interactable.Health -= AttackDamage;

        var lifeSteal = Convert.ToInt32(AttackDamage * Constants.Game.Interactable.Cavalry.LifeStealPercentage * 0.01);
        Health += lifeSteal;
    }
}