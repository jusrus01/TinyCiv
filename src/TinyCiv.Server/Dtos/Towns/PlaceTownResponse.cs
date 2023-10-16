using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Dtos.Towns;

public record PlaceTownResponse(Map? Map, params ServerEvent?[]? Events);