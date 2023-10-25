using System;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.UnitBuilder;

public interface IUnitBuilder
{
    IUnitBuilder SetMaxHealth(int maxHealth);
    IUnitBuilder SetDamage(int damage);
    IUnitBuilder SetSpeed(int speed);
    IUnitBuilder SetProductionPrice(int productionPrice);
    IUnitBuilder SetExpReward(int expReward);
    IUnitBuilder SetDescription(string description);

    // Common GameObject properties
    IUnitBuilder SetImage(string imageSource = null);
    IUnitBuilder SetPosition(Position position);
    IUnitBuilder SetOwnerId(Guid ownerId);
    IUnitBuilder SetId(Guid id);
    IUnitBuilder SetColor(TeamColor color);
    IUnitBuilder SetOpponentId(Guid? opponentId);
    IUnitBuilder SetBorderProperties(BorderProperties borderProperties);

    Unit Build();
}
