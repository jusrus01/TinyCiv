using TinyCiv.Server.Game.Buildings;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.Buildings;

public static class BuildingsMapper
{
    public static Dictionary<BuildingType, IBuilding> Buildings { get; } = new Dictionary<BuildingType, IBuilding>()
    {
        { BuildingType.Farm, new Farm() { IntervalMs = 5000 } },
        { BuildingType.Bank, new Bank() { IntervalMs = 3000 } },
        { BuildingType.Port, new Port() { IntervalMs = 4000 } },
        { BuildingType.Mine, new Mine() { IntervalMs = 4500 } },
        { BuildingType.Shop, new Shop() { IntervalMs = 7000 } },
        { BuildingType.Blacksmith, new Blacksmith() { IntervalMs = 6000 } }
    };
}
