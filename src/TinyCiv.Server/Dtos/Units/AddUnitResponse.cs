using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Dtos.Units;

public record AddUnitResponse(ServerGameObject Unit, Map Map, params ServerEvent?[]? Events);
