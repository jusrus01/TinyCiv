using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Server;

public record JoinLobbyServerEvent(Player Created, string ConnectionId) : ServerEvent;
