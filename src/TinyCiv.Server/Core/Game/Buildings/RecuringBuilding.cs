using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.Buildings
{
    public abstract class RecuringBuilding : IBuilding
    {
        public int Price { get; set; }

        public BuildingType BuildingType { get; set; }

        public GameObjectType TileType { get; set; }

        public int IntervalMs { get; set; }

        //Template method
        public async Task Trigger(Guid playerId, IResourceService resourceService)
        {
            await Delay();
            UpdateResources(playerId, resourceService);
        }

        protected abstract void UpdateResources(Guid playerId, IResourceService resourceService);
        protected abstract Task Delay();
    }
}
