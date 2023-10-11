using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.units
{
    public class Tarran : Unit
    {
        public Tarran(ServerGameObject serverGameObject) : base(serverGameObject)
        {
            MaxHealth = Shared.Constants.Game.Interactable.Tarran.InitialHealth;
            Health = MaxHealth;
            Damage = Shared.Constants.Game.Interactable.Tarran.Damage;
            Speed = 1;
            ProductionPrice = 50;
            ExpReward = 50;
            Description = "5x more damage against the cities and 5x reduced damage from them";
        }
    }
}
