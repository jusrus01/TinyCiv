using TinyCiv.Shared.Dto;

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{
    PlayerDto? AddNewPlayerToGame();
    
    string StartSession();

    bool IsValidPlayer(int playerId);
    bool IsSessionStarted();
    bool AllPlayersInLobby();

    string GetMap();
    void PlaceUnit(int x, int y);
}