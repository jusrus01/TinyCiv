using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableWarrior : IInteractableObject
{
    public int AttackDamage => Constants.Game.Interactable.Warrior.Damage;
    public int AttackRateInMilliseconds => Constants.Game.Interactable.AttackIntervalInMilliseconds;
    public bool IsAbleToCounterAttack => false;
    public bool IsBuilding => false;
    public int Price => Constants.Game.Interactable.Warrior.Price;

    public int? SpawnClonesBeforeDeath { get; set; } = null;
    public int InitialHealth => Constants.Game.Interactable.Warrior.InitialHealth;
    public int Health { get; set; } = Constants.Game.Interactable.Warrior.InitialHealth;
    public Guid GameObjectReferenceId { get; set; }

    public void DoDamage(IInteractableObject interactable)
    {
        if (interactable is InteractableTarran)
        {
            interactable.Health -= AttackDamage * Constants.Game.Interactable.Warrior.TarranDamageReductionMultiplier;
        }
        else
        {
            interactable.Health -= AttackDamage;
        }
    }

    public IInteractableObject Clone()
    {
        return new InteractableWarrior
        {
            GameObjectReferenceId = GameObjectReferenceId,
            Health = Health
        };
    }
}