using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Server;

public record AddNewUnitServerEvent(ServerGameObject CreatedUnit) : ServerEvent;
