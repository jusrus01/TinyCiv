using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units
{
    public class Warrior : Unit
    {
        public Warrior(ServerGameObject serverGameObject) : base(serverGameObject)
        {
            MaxHealth = Shared.Constants.Game.Interactable.Warrior.InitialHealth;
            Health = MaxHealth;
            Damage = Shared.Constants.Game.Interactable.Warrior.Damage;
            Speed = 2;
            ProductionPrice = 50;
            ExpReward = 50;
        }
    }
}
