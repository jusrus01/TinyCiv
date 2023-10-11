using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code.units
{
    public class Tarran : Unit
    {
        public override int MaxHealth => Shared.Constants.Game.Interactable.Tarran.InitialHealth;
        public override int Damage => Shared.Constants.Game.Interactable.Tarran.Damage;
        public override int Speed => 1;
        public override int ProductionPrice => 50;
        public override int ExpReward => 50;
        public override string Description => "5x more damage against the cities and 5x reduced damage from them";

        public Tarran(Unit go) : base(go)
        {
            Health = MaxHealth;
        }       
    }
}
