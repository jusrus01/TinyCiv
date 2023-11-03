namespace TinyCiv.Shared.Events.Server;

public record VictoryServerEvent(Guid PlayerId) : ServerEvent;