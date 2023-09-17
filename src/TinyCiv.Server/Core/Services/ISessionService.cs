using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{    
    Player? AddPlayer();
    Map InitMap();
    
    bool IsLobbyFull();
    bool IsStarted();
}