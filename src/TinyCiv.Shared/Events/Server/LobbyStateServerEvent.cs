namespace TinyCiv.Shared.Events.Server;

public record LobbyStateServerEvent(bool CanGameStart, string ConnectionId) : ServerEvent;