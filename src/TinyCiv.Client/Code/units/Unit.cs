using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units;

public abstract class Unit : GameObject
{
    public int Health { get; set; }
    public abstract int MaxHealth { get; }
    public abstract int Damage { get; }
    public abstract int Speed { get; }
    public abstract int ProductionPrice { get; }
    public abstract int ExpReward { get; }
    public abstract string Description { get; }

    protected Unit(ServerGameObject serverGameObject) : base(serverGameObject)
    {

    }
}
