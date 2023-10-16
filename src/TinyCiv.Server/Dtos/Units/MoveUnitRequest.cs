using TinyCiv.Server.Enums;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Dtos.Units
{
    public record MoveUnitRequest(Guid UnitId, ServerPosition Position, Action<UnitMoveResponse, Map?> UnitMoveCallback);
}
