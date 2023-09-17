<<<<<<< HEAD
using TinyCiv.Shared.Dto;
=======
using TinyCiv.Shared.Game;
>>>>>>> master

namespace TinyCiv.Server.Core.Services;

public interface ISessionService
{
<<<<<<< HEAD
    PlayerDto? AddNewPlayerToGame();
    
    string StartSession();

    bool IsValidPlayer(int playerId);
    bool IsSessionStarted();
    bool AllPlayersInLobby();

    string GetMap();
    void PlaceUnit(int x, int y);
=======
    Player? AddPlayer();
    Map InitMap();
    
    bool IsLobbyFull();
    bool IsStarted();
>>>>>>> master
}