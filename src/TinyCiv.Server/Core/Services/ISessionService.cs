using TinyCiv.Server.Core.Iterators;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{    
    Player? AddPlayer(string connectionId);
    Player GetPlayer(Guid playerId);
    IIterator<Player> GetIterator();

    Player? RemovePlayer(Guid playerId);
    void RemovePlayerByConnectionId(string connectionId);

    void StartGame();
    void StopGame();
    bool IsLobbyFull();
    bool IsStarted();
    bool CanGameStart();
}