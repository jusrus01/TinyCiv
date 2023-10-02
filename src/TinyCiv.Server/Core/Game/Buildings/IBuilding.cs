using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.Buildings;

public interface IBuilding
{
    GameObjectType? TileType { get; }
    int IntervalMs { get; set; }

    void Trigger(Guid playerId, IResourceService resourceService);
}
