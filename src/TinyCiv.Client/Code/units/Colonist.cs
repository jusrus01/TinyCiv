using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.units
{
    public class Colonist : Unit
    {
        public override int MaxHealth => Shared.Constants.Game.Interactable.Colonist.InitialHealth;
        public override int Damage => Shared.Constants.Game.Interactable.Colonist.Damage;
        public override int Speed => 2;
        public override int ProductionPrice => 100;
        public override int ExpReward => 10;
        public override string Description => "Can create a new city";
        public Colonist(GameObject go) : base(go)
        {
            Health = MaxHealth;
        }
    }
}
