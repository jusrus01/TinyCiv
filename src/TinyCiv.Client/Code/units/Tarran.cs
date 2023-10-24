using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.units
{
    public class Tarran : Unit
    {
        private const int InitialMaxHealth = Shared.Constants.Game.Interactable.Tarran.InitialHealth;
        private const int InitialDamage = Shared.Constants.Game.Interactable.Tarran.Damage;
        private const int InitialSpeed = 1;
        private const int InitialProductionPrice = 50;
        private const int InitialExpReward = 50;
        private const string InitialDescription = "5x more damage against the cities and 5x reduced damage from them";

        public override GameObjectType Type => GameObjectType.Tarran;

        public Tarran() : base()
        {
            InitializeDefaults();
        }

        public Tarran(GameObject go) : base(go)
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
            Description = InitialDescription;
            Health = MaxHealth;
        }
    }
}
