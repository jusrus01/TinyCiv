namespace TinyCiv.Shared.Events.Server;

public record GameStartServerEvent(string Map) : ServerEvent;
