namespace TinyCiv.Shared.Events.Server;

public record InteractableObjectServerEvent(Guid ObjectId, int Health, int AttackDamage, string ConnectionId) : ServerEvent;
