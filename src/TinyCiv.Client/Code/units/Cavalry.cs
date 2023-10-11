using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units;

public class Cavalry : Unit
{
    public Cavalry(ServerGameObject serverGameObject) : base(serverGameObject)
    {
        MaxHealth = Shared.Constants.Game.Interactable.Cavalry.InitialHealth;
        Health = MaxHealth;
        Damage = Shared.Constants.Game.Interactable.Cavalry.Damage;
        Speed = 3;
        ProductionPrice = 100;
        ExpReward = 60;
    }
}
