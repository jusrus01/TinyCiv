namespace TinyCiv.Shared.Events.Client;

public record AttackUnitClientEvent(Guid AttackerId, Guid OpponentId) : ClientEvent;