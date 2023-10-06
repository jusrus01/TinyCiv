using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Client;

public record CreateBuildingClientEvent(Guid PlayerId, BuildingType BuildingType, ServerPosition Position) : ClientEvent;