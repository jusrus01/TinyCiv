using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Server;

public record GameStartServerEvent(Map Map, string ConnectionId) : ServerEvent;
