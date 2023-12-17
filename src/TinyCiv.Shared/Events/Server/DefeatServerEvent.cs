namespace TinyCiv.Shared.Events.Server;

public record DefeatServerEvent(Guid PlayerId, string ConnectionId) : ServerEvent;