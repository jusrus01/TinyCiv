using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IMapService
{
    void AddUnit(Guid playerId, Position position);
    
    Map Initialize();
    Map? GetMap();
}
