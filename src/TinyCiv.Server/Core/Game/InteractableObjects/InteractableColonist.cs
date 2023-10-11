using TinyCiv.Shared;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public class InteractableColonist : IInteractableObject
{
    public int Health { get; set; } = Constants.Game.Interactable.Colonist.InitialHealth;
    public int AttackDamage { get; set; } = Constants.Game.Interactable.Colonist.Damage;
    public int AttackRateInMilliseconds { get; set; } = Constants.Game.Interactable.AttackIntervalInMilliseconds;
}