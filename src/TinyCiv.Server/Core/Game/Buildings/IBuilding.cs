using TinyCiv.Server.Core.Services;

namespace TinyCiv.Server.Core.Game.Buildings;

public interface IBuilding
{
    int IntervalMs { get; set; }

    void Trigger(Guid playerId, IResourceService resourceService);
}
