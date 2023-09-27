using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Entities;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class ResourceService : IResourceService
{
    private readonly List<PlayerResources> _resources;

    public ResourceService()
    {
        _resources = new List<PlayerResources>();
    }

    public void AddBuilding(Guid playerId, IBuilding building)
    {
        Task.Run(async () =>
        {
            await Task.Delay(building.IntervalMs);
            building.Trigger(playerId, this);
            // How to send update from here?
        });
    }

    public void AddResources(Guid PlayerId, ResourceType resourceType, int amount)
    {
        var resourceEntry = _resources
            .Where(r => r.PlayerId == PlayerId)
            .SingleOrDefault();

        if (resourceEntry == null)
        {
            resourceEntry = new PlayerResources() { PlayerId = PlayerId };
            _resources.Add(resourceEntry);
        }

        resourceEntry.AddResource(resourceType, amount);
    }
}
