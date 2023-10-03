namespace TinyCiv.Shared.Events.Client;

public record AttackUnitClientEvent(Guid OpponentId) : ClientEvent;