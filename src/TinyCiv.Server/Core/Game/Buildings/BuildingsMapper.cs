using System.Reflection.Metadata;
using TinyCiv.Server.Game.Buildings;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.Buildings;

public static class BuildingsMapper
{
    public static Dictionary<BuildingType, IBuilding> Buildings { get; } = new Dictionary<BuildingType, IBuilding>()
    {
        { BuildingType.Farm, new Farm() { IntervalMs = Constants.Game.FarmInterval } },
        { BuildingType.Bank, new Bank() { IntervalMs = Constants.Game.BankInterval } },
        { BuildingType.Port, new Port() { IntervalMs = Constants.Game.PortInterval } },
        { BuildingType.Mine, new Mine() { IntervalMs = Constants.Game.MineInterval } },
        { BuildingType.Shop, new Shop() { IntervalMs = Constants.Game.ShopInterval } },
        { BuildingType.Blacksmith, new Blacksmith() { IntervalMs = Constants.Game.BlacksmithInterval } }
    };
}
