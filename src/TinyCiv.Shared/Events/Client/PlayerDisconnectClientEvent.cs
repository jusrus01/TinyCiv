namespace TinyCiv.Shared.Events.Client;

// Requires implementation
public record PlayerDisconnectClientEvent(Guid PlayerId) : ClientEvent;
