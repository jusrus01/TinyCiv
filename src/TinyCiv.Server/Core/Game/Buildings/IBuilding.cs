using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.Buildings;

public interface IBuilding
{
    int Price { get; }
    BuildingType BuildingType { get; }
    GameObjectType TileType { get; }
    int IntervalMs { get; set; }

    Task Trigger(Guid playerId, IResourceService resourceService);
}
