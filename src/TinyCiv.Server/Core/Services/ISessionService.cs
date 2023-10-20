using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{    
    Player? AddPlayer(string connectionId);
    Player GetPlayer(Guid playerId);
    List<Player> GetPlayers();
    void RemovePlayerByConnectionId(string connectionId);

    void StartGame();
    bool IsLobbyFull();
    bool IsStarted();
    bool CanGameStart();
}