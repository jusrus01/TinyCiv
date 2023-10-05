using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Enums;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IMapService
{
    ServerGameObject? CreateUnit(Guid playerId, ServerPosition position);
    ServerGameObject? GetUnit(ServerPosition position);
    ServerGameObject? GetUnit(Guid? unitId);

    void ReplaceWithEmpty(Guid id);
    
    Task MoveUnitAsync(Guid unitId, ServerPosition position, Action<UnitMoveResponse> unitMoveCallback);
    ServerGameObject? CreateBuilding(Guid PlayerId,  ServerPosition position, IBuilding building);

    Map? Initialize(MapType mapType);
    Map? GetMap();
}
