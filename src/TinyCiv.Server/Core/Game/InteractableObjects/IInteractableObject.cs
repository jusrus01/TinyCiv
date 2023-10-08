namespace TinyCiv.Server.Core.Game.InteractableObjects;

public interface IInteractableObject
{
    public int Health { get; set; }
    public int AttackDamage { get; set; }
    public int AttackRateInMilliseconds { get; set; }
}