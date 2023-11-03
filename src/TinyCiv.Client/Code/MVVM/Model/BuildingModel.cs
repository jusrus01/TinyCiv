using System.Windows.Navigation;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.Model;

public class BuildingModel : IBuyable
{
    public string ImagePath { get; }
    public string Name { get; }
    public string Production { get; }
    public int IndustryCost { get; }
    public TeamColor Color { get; }
    public GameObjectType Type { get; }

    public BuildingModel(string production, int cost, TeamColor color, GameObjectType type)
    {
        Production = production;
        IndustryCost = cost;
        Color = color;
        Type = type;
        Name = type.ToString();
        ImagePath = AbstractGameObjectFactory.getGameObjectImage(color, type);
    }

    public bool IsBuyable()
    {
        return CurrentPlayer.Instance.Resources.Industry >= IndustryCost;
    }
}
