using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableCavalry : IInteractableObject
{
    public int AttackDamage => Constants.Game.Interactable.Cavalry.Damage;
    public int AttackRateInMilliseconds => Constants.Game.Interactable.AttackIntervalInMilliseconds;
    public bool IsAbleToCounterAttack => false;
    public bool IsBuilding => false;
    public int? SpawnClonesBeforeDeath { get; set; } = null;
    public int Price => Constants.Game.Interactable.Cavalry.Price;

    public int InitialHealth => Constants.Game.Interactable.Cavalry.InitialHealth;
    public int Health { get; set; } = Constants.Game.Interactable.Cavalry.InitialHealth;
    public Guid GameObjectReferenceId { get; set; }
    
    public void DoDamage(IInteractableObject interactable)
    {
        interactable.Health -= AttackDamage;

        var lifeSteal = Convert.ToInt32(AttackDamage * Constants.Game.Interactable.Cavalry.LifeStealPercentage * 0.01);
        Health += lifeSteal;
    }

    public IInteractableObject Clone()
    {
        return new InteractableCavalry
        {
            Health = Health,
            GameObjectReferenceId = GameObjectReferenceId,
            SpawnClonesBeforeDeath = null
        };
    }
}