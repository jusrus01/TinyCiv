using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IMapReader
{
    IList<string> Read(MapType type);
}