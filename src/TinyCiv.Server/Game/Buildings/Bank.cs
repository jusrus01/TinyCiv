using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings
{
    public class Bank : IBuilding
    {
        public GameObjectType? TileType { get; }
        public int IntervalMs { get; set; }

        public void Trigger(Guid playerId, IResourceService resourceService)
        {
            resourceService.AddResources(playerId, ResourceType.Gold, 5);
            Console.WriteLine($"Building \"{GetType()}\" generated 5 Gold for player: {playerId}");
        }
    }
}
