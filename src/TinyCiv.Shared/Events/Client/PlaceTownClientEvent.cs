namespace TinyCiv.Shared.Events.Client;

public record PlaceTownClientEvent(Guid PlayerId) : ClientEvent;
