using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Entities.GameStates;
using TinyCiv.Shared;

namespace TinyCiv.Server.Services;

public class GameStateService : IGameStateService
{
    private IGameState _state;
    private TimeOnly _lastGameStateChange;

    public GameStateService()
    {
        _state = new NotStartedState();
        _lastGameStateChange = TimeOnly.FromDateTime(DateTime.Now);
    }

    public IGameState GetState()
    {
        return _state;
    }

    public bool SetState(IGameState gameState)
    {
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        if ((currentTime - _lastGameStateChange).Milliseconds >= Constants.Game.GameModeAbilityDurationMs)
        {
            _state = gameState;
            _lastGameStateChange = currentTime;
            return true;
        }

        return false;
    }

    public void ResetState()
    {
        _state = new NormalState();
    }
}
