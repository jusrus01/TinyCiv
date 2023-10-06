using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Entities;

public class PlayerResources
{
    public Guid PlayerId { get; init; }
    public int Food { get; private set; } = 0;
    public int Industry { get; private set; } = 0;
    public int Gold { get; private set; } = 0;

    public void AddResource(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Food:
                Food += amount;
                break;
            case ResourceType.Industry:
                Industry += amount;
                break;
            case ResourceType.Gold:
                Gold += amount;
                break;
        }
    }

    public Resources GetResources()
    {
        // Questionable
        return new Resources { Food = Food, Gold = Gold, Industry = Industry };
    }
}
