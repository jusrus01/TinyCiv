using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{    
    Player? AddPlayer();

    void StartGame();
    bool IsLobbyFull();
    bool IsStarted();
}