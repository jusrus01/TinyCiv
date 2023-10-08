using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Client;

public record CreateUnitClientEvent(Guid PlayerId, int X, int Y, GameObjectType UnitType = GameObjectType.Warrior) : ClientEvent;
