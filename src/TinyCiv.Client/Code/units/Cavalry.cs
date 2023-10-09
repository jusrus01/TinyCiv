using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units;

public class Cavalry : Unit
{
    public override int Health => 60;
    public override int MaxHealth => 60;
    public override int Damage => 30;
    public override int Speed => 3;
    public override int ProductionPrice => 100;
    public override int ExpReward => 60;
    public override string Description => null;

    public Cavalry(ServerGameObject serverGameObject) : base(serverGameObject)
    {
    }
}
