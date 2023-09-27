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

    // Init player's resources

    public void AddBuilding(Guid playerId, IBuilding building, Action<Resources> callback)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(building.IntervalMs);
                building.Trigger(playerId, this);

                var playerResources = _resources
                    .Where(r => r.PlayerId == playerId)
                    .SingleOrDefault();

                callback?.Invoke(playerResources!.GetResources());
            }
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
