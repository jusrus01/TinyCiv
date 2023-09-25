using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{    
    Player? AddPlayer();
    Player GetPlayer(Guid playerId);

    void StartGame();
    bool IsLobbyFull();
    bool IsStarted();
    bool CanGameStart();
}