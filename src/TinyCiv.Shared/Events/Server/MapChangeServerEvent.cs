using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Server;

public record MapChangeServerEvent(Map Map) : ServerEvent;
