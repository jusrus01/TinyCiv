using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Entities.GameStates;
using TinyCiv.Shared;

namespace TinyCiv.Server.Services;

public class GameStateService : IGameStateService
{
    private IGameState _state;
    private TimeOnly _lastGameStateChange;
    private Dictionary<IGameState, List<Guid>> _usedAbilities = new();

    public GameStateService()
    {
        _state = new NotStartedState();
        _lastGameStateChange = TimeOnly.FromDateTime(DateTime.Now);
        _usedAbilities = new Dictionary<IGameState, List<Guid>>
        {
            { new RestrictedState(), new() },
            { new OnlyBuildingState(), new() },
            { new OnlyUnitState(), new() }
        };
    }

    public IGameState GetState()
    {
        return _state;
    }

    public bool SetState(Guid playerId, IGameState gameState)
    {
        if (_usedAbilities[gameState].Contains(playerId))
        {
            return false;
        }

        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        if ((currentTime - _lastGameStateChange).Milliseconds >= Constants.Game.GameModeAbilityDurationMs)
        {
            _usedAbilities[gameState].Add(playerId);
            SetStateInstant(gameState);
            _lastGameStateChange = currentTime;
            return true;
        }

        return false;
    }

    public void SetStateInstant(IGameState gameState)
    {
        Console.WriteLine(gameState.ToString());
        _state = gameState;
    }
}
