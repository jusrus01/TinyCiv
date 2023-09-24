namespace TinyCiv.Shared.Events.Client;

// Requires implementation
public record MoveUnitClientEvent(Guid UnitId, int X, int Y) : ClientEvent;
