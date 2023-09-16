namespace TinyCiv.Shared.Events.Client;

public record AddNewUnitClientEvent(Guid PlayerId, int X, int Y) : ClientEvent;
