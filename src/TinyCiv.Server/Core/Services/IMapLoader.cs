using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IMapLoader
{
    List<ServerGameObject> Load(MapType type);
}