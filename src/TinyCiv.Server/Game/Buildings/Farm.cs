using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings;

public class Farm : IBuilding
{
    public int IntervalMs { get; set; }

    public void Trigger(Guid playerId, IResourceService resourceService)
    {
        resourceService.AddResources(playerId, ResourceType.Food, 2);
        Console.WriteLine($"Building \"{GetType()}\" generated 2 Food for player: {playerId}");
    }
}
