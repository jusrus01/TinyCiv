using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Enums;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IMapService
{
    ServerGameObject? CreateUnit(Guid playerId, ServerPosition position, GameObjectType type);
    ServerPosition? TryFindClosestAvailablePosition(ServerPosition? target);
    ServerGameObject? GetUnit(ServerPosition position);
    ServerGameObject? GetUnit(Guid? unitId);

    ServerGameObject? PlaceTown(Guid playerId);
    bool IsTownOwner(Guid playerId);
    bool IsInRange(ServerPosition position, int range, GameObjectType type);

    void ReplaceWithEmpty(Guid id);

    Task MoveUnitAsync(Guid unitId, ServerPosition position, Action<UnitMoveResponse> unitMoveCallback);
    ServerGameObject? CreateBuilding(Guid playerId, ServerPosition position, IBuilding building);

    Map? Initialize(MapType mapType);
    Map? GetMap();
    IList<ServerGameObject> GetMapObjects();
}
