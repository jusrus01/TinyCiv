namespace TinyCiv.Shared.Events.Client;

public record CreateUnitClientEvent(Guid PlayerId, int X, int Y) : ClientEvent;
