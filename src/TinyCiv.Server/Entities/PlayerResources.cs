using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Entities;

public class PlayerResources
{
    private readonly Dictionary<ResourceType, int> _resources = new()
    {
        { ResourceType.Food, 0 },
        { ResourceType.Industry, 0 },
        { ResourceType.Gold, 0 },
    };
    
    public Guid PlayerId { get; init; }

    public bool DecreaseResource(ResourceType type, int amount)
    {
        EnsureResourceTypePresent(type);

        var decreasedAmount = _resources[type] - amount;
        if (decreasedAmount < 0)
        {
            return false;
        }

        _resources[type] = decreasedAmount;
        return true;
    }
    
    public void AddResource(ResourceType type, int amount)
    {
        EnsureResourceTypePresent(type);
        _resources[type] += amount;
    }

    private void EnsureResourceTypePresent(ResourceType type)
    {
        if (!_resources.ContainsKey(type))
        {
            throw new InvalidOperationException();
        }
    }

    public Resources GetResources()
    {
        return new Resources
        {
            Food = _resources[ResourceType.Food],
            Gold = _resources[ResourceType.Gold],
            Industry = _resources[ResourceType.Industry]
        };
    }
}
