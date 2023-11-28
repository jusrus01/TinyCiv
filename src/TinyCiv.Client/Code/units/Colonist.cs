using System.Windows;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.units
{
    public class Colonist : Unit
    {
        private const int InitialMaxHealth = 0;
        private const int InitialDamage = 0;
        private const int InitialSpeed = 2;
        private const int InitialProductionPrice = 100;
        private const int InitialExpReward = 10;
        private const string InitialDescription = "Can create a new city";

        public override GameObjectType Type => GameObjectType.Colonist;

        public Colonist() : base()
        {
            InitializeDefaults();
        }

        public Colonist(GameObject go) : base(go)
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

        public void SettleDown()
        {
            ClientSingleton.Instance.serverClient.SendAsync(new PlaceTownClientEvent(CurrentPlayer.Id));
            HUDManager.Instance.HideLowerMenu();
        }
    }
}
