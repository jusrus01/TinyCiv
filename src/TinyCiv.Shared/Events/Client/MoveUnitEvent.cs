namespace TinyCiv.Shared.Events.Client;

//  Requires implementation
public record MoveUnitEvent(Guid UnitId, int X, int Y) : ClientEvent;
