using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.units
{
    public class Colonist : Unit
    {    
        public Colonist(ServerGameObject serverGameObject) : base(serverGameObject)
        {
            MaxHealth = Shared.Constants.Game.Interactable.Colonist.InitialHealth;
            Health = MaxHealth;
            Damage = Shared.Constants.Game.Interactable.Colonist.Damage;
            Speed = 2;
            ProductionPrice = 100;
            ExpReward = 10;
            Description = "Can create a new city";
        }
    }
}
