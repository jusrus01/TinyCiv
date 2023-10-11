namespace TinyCiv.Client.Code.Units;

public abstract class Unit : GameObject
{
    public int Health { get; set; }
    public virtual int MaxHealth { get; }
    public virtual int Damage { get; }
    public virtual int Speed { get; }
    public virtual int ProductionPrice { get; }
    public virtual int ExpReward { get; }
    public virtual string Description { get; }

    protected Unit(Unit unit) : base(unit.Type, unit.Position, unit.OwnerId, unit.Id, unit.Color, unit.OpponentId)
    {
        Health = unit.Health;
    }

    protected Unit(GameObject go) : base(go.Type, go.Position, go.OwnerId, go.Id, go.Color, go.OpponentId)
    {

    }
}
