using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM;

public class BuildingModel
{
    public string ImagePath { get; }
    public string Name { get; }
    public string Production {  get; }
    public string Cost {  get; }
    public TeamColor Color { get; }
    public GameObjectType Type { get; }

    public BuildingModel(string production, string cost, TeamColor color, GameObjectType type)
    {
        Production = production;
        Cost = cost;
        Color = color;
        Type = type;
        Name = type.ToString();
        ImagePath = Images.GetImage(Color, Type);
    }
}
