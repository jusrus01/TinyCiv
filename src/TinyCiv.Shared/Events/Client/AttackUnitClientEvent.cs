namespace TinyCiv.Shared.Events.Client;

public record AttackUnitClientEvent(Guid? PlayerId, Guid? AttackerId, Guid? OpponentId) : ClientEvent;