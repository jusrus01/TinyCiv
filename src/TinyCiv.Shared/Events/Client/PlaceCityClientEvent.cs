namespace TinyCiv.Shared.Events.Client;

public record PlaceCityClientEvent(Guid PlayerId) : ClientEvent;
