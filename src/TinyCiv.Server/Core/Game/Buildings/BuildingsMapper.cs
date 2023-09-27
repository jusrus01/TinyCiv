using TinyCiv.Server.Game.Buildings;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.Buildings;

public static class BuildingsMapper
{
    public static Dictionary<BuildingType, IBuilding> Buildings { get; } = new Dictionary<BuildingType, IBuilding>()
    {
        { BuildingType.Farm, new Farm() { IntervalMs = 5000 } }
    };
}
