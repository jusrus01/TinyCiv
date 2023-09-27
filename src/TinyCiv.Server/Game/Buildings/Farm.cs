using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings;

public class Farm : IBuilding
{
    public int IntervalMs { get; set; } = 5000;

    public void Trigger(Guid playerId, IResourceService resourceService)
    {
        // Kai kurie buildingai naudoja kitus resursus, pvz Farm naudoja 1 goldo kad padarytu 5 maisto
        // Turi galimybe pasiupgradint laiko intervala (random sansas arba po kazkiek sugeneruotu resursu)
        resourceService.AddResources(playerId, ResourceType.Food, 2);
        Console.WriteLine($"Generated 2 Food for player");
    }
}
