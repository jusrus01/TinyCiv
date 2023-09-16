namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{
    Guid? AddNewPlayerToGame();
    
    string StartSession();

    bool IsValidPlayer(Guid playerId);
    bool IsSessionStarted();
    bool AllPlayersInLobby();

    string GetMap();
    void PlaceUnit(int x, int y);
}