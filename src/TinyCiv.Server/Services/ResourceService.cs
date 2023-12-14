using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Entities;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;
using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Server.Visitor;

namespace TinyCiv.Server.Services;

public class ResourceService : IResourceService
{
    private readonly List<PlayerResources> _resources;
    private readonly object _resourceLocker;
    private readonly List<IVisitorElement> _visitorBuildings;

    public ResourceService()
    {
        _resources = new List<PlayerResources>();
        _resourceLocker = new object();
        _visitorBuildings = new List<IVisitorElement>();
    }

    public Resources InitializeResources(Guid playerId)
    {
        lock (_resourceLocker)
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
        _visitorBuildings.Add((IVisitorElement)building);

        RandomizeIntervals();
        PrintStatistics();

        Task.Run(async () =>
        {
            while (true)
            {
                await building.Trigger(playerId, this);

                var playerResources = _resources
                    .Where(r => r.PlayerId == playerId)
                    .Single();

                callback?.Invoke(playerResources!.GetResources());
            }
        });
    }

    private void PrintStatistics()
    {
        StatsVisitor statsVisitor = new();
        Console.WriteLine("\nExisting buildings");
        _visitorBuildings.ForEach(b => b.Accept(statsVisitor));
        Console.WriteLine();
    }

    private void RandomizeIntervals()
    {
        ModsVisitor modsVisitor = new();
        _visitorBuildings.ForEach(b => b.Accept(modsVisitor));
    }

    public void AddResources(Guid PlayerId, ResourceType resourceType, int amount)
    {
        lock (_resourceLocker)
        {
            var resourceEntry = _resources
                .Where(r => r.PlayerId == PlayerId)
                .Single();

            resourceEntry.AddResource(resourceType, amount);
        }
    }

    public Resources GetResources(Guid playerId)
    {
        lock (_resourceLocker)
        {
            return _resources
                .Where(r => r.PlayerId == playerId)
                .Single()
                .GetResources();
        }
    }

    public Resources? BuyInteractable(Guid playerId, IInteractableInfo info)
    {
        lock (_resourceLocker)
        {
            var playerResource = _resources.SingleOrDefault(player => player.PlayerId == playerId);
            if (playerResource == null)
            {
                return null;
            }

            var isPurchaseSuccessful = playerResource.DecreaseResource(ResourceType.Gold, info.Price);
            
            return isPurchaseSuccessful ? playerResource.GetResources() : null;
        }
    }

    public void CancelInteractablePayment(Guid playerId, IInteractableInfo? info)
    {
        if (info == null)
        {
            return;
        }

        lock (_resourceLocker)
        {
            var playerResource = _resources.SingleOrDefault(player => player.PlayerId == playerId);
            if (playerResource == null)
            {
                return;
            }
            
            playerResource.AddResource(ResourceType.Gold, info.Price);
        }
    }
}
