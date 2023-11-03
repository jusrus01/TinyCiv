namespace TinyCiv.Shared.Events.Server;

public record DefeatServerEvent(Guid PlayerId) : ServerEvent;