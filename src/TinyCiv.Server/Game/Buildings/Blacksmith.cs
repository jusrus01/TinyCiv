using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings
{
    public class Blacksmith : IBuilding
    {
        public int IntervalMs { get; set; }

        public void Trigger(Guid playerId, IResourceService resourceService)
        {
            resourceService.AddResources(playerId, ResourceType.Industry, 5);
            resourceService.AddResources(playerId, ResourceType.Gold, -1);
            Console.WriteLine($"Building \"{GetType()}\" generated 5 Industry in exchange for 1 Gold for player: {playerId}");
        }
    }
}
