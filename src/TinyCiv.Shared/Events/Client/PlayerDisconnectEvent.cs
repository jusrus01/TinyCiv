namespace TinyCiv.Shared.Events.Client;

// Requires implementation
public record PlayerDisconnectEvent(Guid playerId) : ClientEvent;
