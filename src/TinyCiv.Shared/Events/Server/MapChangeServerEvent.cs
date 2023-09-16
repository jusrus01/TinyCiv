namespace TinyCiv.Shared.Events.Server;

public record MapChangeServerEvent(string Map) : ServerEvent;
