using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings
{
    public class Mine : IBuilding
    {
        public int Price { get; }
        public BuildingType BuildingType { get; }
        public GameObjectType TileType { get; }
        public int IntervalMs { get; set; }

        public Mine()
        {
            Price = Constants.Game.MinePrice;
            BuildingType = BuildingType.Mine;
            TileType = GameObjectType.StaticMountain;
            IntervalMs = Constants.Game.MineInterval;
        }

        public void Trigger(Guid playerId, IResourceService resourceService)
        {
            int amount = new Random().Next(1, 5);
            resourceService.AddResources(playerId, ResourceType.Industry, amount);
            Console.WriteLine($"Building \"{GetType()}\" generated {amount} Industry for player: {playerId}");
        }
    }
}
