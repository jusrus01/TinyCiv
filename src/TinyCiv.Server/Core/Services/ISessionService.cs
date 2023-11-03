using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{    
    Player? AddPlayer(string connectionId);
    Player GetPlayer(Guid playerId);
    List<Player> GetPlayers();
    List<Guid> GetPlayerIds();

    Player? RemovePlayer(Guid playerId);
    void RemovePlayerByConnectionId(string connectionId);

    void StartGame();
    void StopGame();
    bool IsLobbyFull();
    bool IsStarted();
    bool CanGameStart();
}