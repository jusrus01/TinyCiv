using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units
{
    public class Warrior : Unit
    {
        private const int InitialMaxHealth = Shared.Constants.Game.Interactable.Warrior.InitialHealth;
        private const int InitialDamage = Shared.Constants.Game.Interactable.Warrior.Damage;
        private const int InitialSpeed = 2;
        private const int InitialProductionPrice = 50;
        private const int InitialExpReward = 50;

        public override GameObjectType Type => GameObjectType.Warrior;

        public Warrior() : base()
        {
            InitializeDefaults();
        }

        public Warrior(GameObject go) : base(go)
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            MaxHealth = InitialMaxHealth;
            Damage = InitialDamage;
            Speed = InitialSpeed;
            ProductionPrice = InitialProductionPrice;
            ExpReward = InitialExpReward;
            Health = MaxHealth;
        }
    }

}
