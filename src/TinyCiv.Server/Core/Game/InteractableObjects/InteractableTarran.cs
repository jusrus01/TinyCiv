using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableTarran : IInteractableObject
{
    public int Health { get; set; } = Constants.Game.Interactable.Tarran.InitialHealth;
    public int AttackDamage { get; set; } = Constants.Game.Interactable.Tarran.Damage;
    public int AttackRateInMilliseconds { get; set; } = Constants.Game.Interactable.AttackIntervalInMilliseconds;
}