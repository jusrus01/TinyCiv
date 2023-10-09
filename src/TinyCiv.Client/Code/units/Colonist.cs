using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.units
{
    public class Colonist : Unit
    {    
        public override int Health => 10;
        public override int MaxHealth => 10;
        public override int Damage => 0;
        public override int Speed => 2;
        public override int ProductionPrice => 100;
        public override int ExpReward => 10;
        public override string Description => "Can create a new city";

        public Colonist(ServerGameObject serverGameObject) : base(serverGameObject)
        {
        }
    }
}
