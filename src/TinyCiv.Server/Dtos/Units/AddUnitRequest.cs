using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Dtos.Units;

public record AddUnitRequest(Guid PlayerId, ServerPosition Position, GameObjectType UnitType);