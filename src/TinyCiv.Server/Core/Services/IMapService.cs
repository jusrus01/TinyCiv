using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IMapService
{
    List<(Guid, ServerPosition)> MovingUnits { get; set; }

    ServerGameObject AddUnit(Guid playerId, ServerPosition position);
    void MoveUnit(Guid unitId, ServerPosition position);

    ServerGameObject GetUnit(Guid unitId);
    ServerGameObject GetUnit(ServerPosition position);
    Map Initialize();
    Map? GetMap();
}
