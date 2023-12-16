using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Server;

public record GameModeChangeServerEvent(GameModeType GameModeType, string ConnectionId) : ServerEvent;