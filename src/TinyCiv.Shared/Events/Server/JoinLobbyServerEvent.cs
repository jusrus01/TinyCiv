namespace TinyCiv.Shared.Events.Server;

public record JoinLobbyServerEvent(Guid AssignedPlayerId) : ServerEvent;
