using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableCity : IInteractableObject
{
    public int Price => 0;
    public int AttackDamage => Constants.Game.Interactable.City.Damage;
    public int AttackRateInMilliseconds => Constants.Game.Interactable.AttackIntervalInMilliseconds;
    public bool IsAbleToCounterAttack => true;
    public bool IsBuilding => true;
    public int? SpawnClonesBeforeDeath { get; set; } = null;

    public Guid GameObjectReferenceId { get; set; }
    public int InitialHealth => Constants.Game.Interactable.City.InitialHealth;
    public int Health { get; set; } = Constants.Game.Interactable.City.InitialHealth;
    
    public void DoDamage(IInteractableObject interactable)
    {
        interactable.Health -= AttackDamage;
    }

    public IInteractableObject Clone()
    {
        return new InteractableCity
        {
            GameObjectReferenceId = GameObjectReferenceId,
            Health = Health,
            SpawnClonesBeforeDeath = null
        };
    }
}