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

    public void InitializeResources(Guid playerId)
    {
        var resourceEntry = new PlayerResources() { PlayerId = playerId };
        _resources.Add(resourceEntry);
    }

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
                    .Single();

                callback?.Invoke(playerResources!.GetResources());
            }
        });
    }

    public void AddResources(Guid PlayerId, ResourceType resourceType, int amount)
    {
        var resourceEntry = _resources
            .Where(r => r.PlayerId == PlayerId)
            .Single();

        resourceEntry.AddResource(resourceType, amount);
    }

    public Resources GetResources(Guid playerId)
    {
        return _resources
            .Where(r => r.PlayerId == playerId)
            .Single()
            .GetResources();
    }
}
