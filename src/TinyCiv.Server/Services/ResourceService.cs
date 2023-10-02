using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Entities;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;

namespace TinyCiv.Server.Services;

public class ResourceService : IResourceService
{
    private readonly List<PlayerResources> _resources;
    private readonly object _resourceLocker;

    public ResourceService()
    {
        _resources = new List<PlayerResources>();
        _resourceLocker = new object();
    }

    public Resources InitializeResources(Guid playerId)
    {
        lock (_resources)
        {
            var resourceEntry = new PlayerResources() { PlayerId = playerId };
            resourceEntry.AddResource(ResourceType.Industry, Constants.Game.StartingIndustry);
            resourceEntry.AddResource(ResourceType.Gold, Constants.Game.StartingGold);
            resourceEntry.AddResource(ResourceType.Food, Constants.Game.StartingFood);
            _resources.Add(resourceEntry);
            return resourceEntry.GetResources();
        }
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
        lock (_resources)
        {
            var resourceEntry = _resources
                .Where(r => r.PlayerId == PlayerId)
                .Single();

            resourceEntry.AddResource(resourceType, amount);
        }
    }

    public Resources GetResources(Guid playerId)
    {
        lock (_resources)
        {
            return _resources
                .Where(r => r.PlayerId == playerId)
                .Single()
                .GetResources();
        }
    }
}
