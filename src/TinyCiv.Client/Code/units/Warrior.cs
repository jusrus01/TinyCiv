using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units
{
    public class Warrior : Unit
    {
        public override int Health => 40;
        public override int MaxHealth => 40;
        public override int Damage => 20;
        public override int Speed => 2;
        public override int ProductionPrice => 50;
        public override int ExpReward => 50;
        public override string Description => null;

        public Warrior(ServerGameObject serverGameObject) : base(serverGameObject)
        {
        }

        
    }
}
