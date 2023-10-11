using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units;

public abstract class Unit : GameObject
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Damage { get; set; }
    public int Speed { get; set; }
    public int ProductionPrice { get; set; }
    public int ExpReward { get; set; }
    public string Description { get; set; }

    protected Unit(ServerGameObject serverGameObject) : base(serverGameObject)
    {

    }
}
