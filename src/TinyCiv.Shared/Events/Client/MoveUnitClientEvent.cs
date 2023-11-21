namespace TinyCiv.Shared.Events.Client;

public record MoveUnitClientEvent(Guid? PlayerId, Guid? UnitId, int X, int Y) : ClientEvent;
