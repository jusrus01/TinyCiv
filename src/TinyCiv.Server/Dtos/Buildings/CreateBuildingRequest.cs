using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Dtos.Buildings;

public record CreateBuildingRequest(Guid PlayerId, BuildingType BuildingType, ServerPosition Position, Action<Resources> ResourceUpdateCallback);
