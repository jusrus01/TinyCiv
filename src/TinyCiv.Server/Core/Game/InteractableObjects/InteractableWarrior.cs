using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableWarrior : IInteractableObject
{
    public int Health { get; set; } = Constants.Game.Interactable.Warrior.InitialHealth;
    public int AttackDamage { get; set; } = Constants.Game.Interactable.Warrior.Damage;
    public int AttackRateInMilliseconds { get; set; } = Constants.Game.Interactable.AttackIntervalInMilliseconds;
}