using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units
{
    public class Warrior : Unit
    {
        public override int MaxHealth => Shared.Constants.Game.Interactable.Warrior.InitialHealth;
        public override int Damage => Shared.Constants.Game.Interactable.Warrior.Damage;
        public override int Speed => 2;
        public override int ProductionPrice => 50;
        public override int ExpReward => 50;

        public Warrior(GameObject go) : base(go)
        {
            Health = MaxHealth;
        }
    }
}
