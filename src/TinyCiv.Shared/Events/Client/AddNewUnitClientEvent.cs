namespace TinyCiv.Shared.Events.Client;

public record AddNewUnitClientEvent(int PlayerId, int X, int Y) : ClientEvent;
