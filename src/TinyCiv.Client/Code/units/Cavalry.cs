using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units;

public class Cavalry : Unit
{
    private const int InitialMaxHealth = Shared.Constants.Game.Interactable.Cavalry.InitialHealth;
    private const int InitialDamage = Shared.Constants.Game.Interactable.Cavalry.Damage;
    private const int InitialSpeed = 3;
    private const int InitialProductionPrice = 100;
    private const int InitialExpReward = 60;
    private const string InitialDescription = null;

    public override GameObjectType Type => GameObjectType.Cavalry;

    public Cavalry() : base()
    {
        InitializeDefaults();
    }

    public Cavalry(GameObject go) : base(go)
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
