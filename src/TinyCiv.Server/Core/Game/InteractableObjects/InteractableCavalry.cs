using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableCavalry : IInteractableObject
{
    public int Health { get; set; } = Constants.Game.Interactable.Cavalry.InitialHealth;
    public int AttackDamage { get; set; } = Constants.Game.Interactable.Cavalry.Damage;
    public int AttackRateInMilliseconds { get; set; } = Constants.Game.Interactable.AttackIntervalInMilliseconds;
}