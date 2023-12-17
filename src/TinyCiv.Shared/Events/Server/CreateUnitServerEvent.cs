using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Server;

public record CreateUnitServerEvent(ServerGameObject CreatedUnit, string ConnectionId) : ServerEvent;
