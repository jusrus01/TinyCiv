namespace TinyCiv.Shared.Events.Server;

public record CreateInteractableObjectServerEvent(Guid ObjectId, int Health, int AttackDamage) : ServerEvent;
