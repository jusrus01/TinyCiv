using TinyCiv.Server.Enums;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IMapService
{
    ServerGameObject? CreateUnit(Guid playerId, ServerPosition position);
    ServerGameObject? GetUnit(ServerPosition position);
    ServerGameObject? GetUnit(Guid unitId);
    Task MoveUnitAsync(Guid unitId, ServerPosition position, Action<UnitMoveResponse> unitMoveCallback);

    ServerGameObject? CreateBuilding(Guid PlayerId,  ServerPosition position);

    Map? Initialize();
    Map? GetMap();
}
