namespace TinyCiv.Shared.Events.Client;

//  Requires implementation
public record MoveUnitEvent(Guid unitId, int newRow, int newColumn) : ClientEvent;
