using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Entities.GameStates;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class GameStateService : IGameStateService
{
    private IGameState _state;
    private int _lastGameStateChange;
    private Guid _lastAbilityUser;
    private Dictionary<string, List<Guid>> _usedAbilities = new();
    private object _gameStateLock = new();

    public GameStateService()
    {
        lock (_gameStateLock)
        {
            _state = new NotStartedState();
            _lastAbilityUser = Guid.NewGuid();
            _lastGameStateChange = int.MinValue / 2;
            _usedAbilities = new Dictionary<string, List<Guid>>
            {
                { nameof(RestrictedState), new() },
                { nameof(OnlyBuildingState), new() },
                { nameof(OnlyUnitState), new() }
            };
        }
    }

    public IGameState GetState()
    {
        return _state;
    }

    public bool SetState(Guid playerId, IGameState gameState)
    {
        lock (_gameStateLock)
        {
            var gameStateName = gameState.GetType().Name;

            if (_usedAbilities[gameStateName].Contains(playerId))
            {
                return false;
            }

            var currentTime = DateTime.Now.Millisecond;
            if (currentTime - _lastGameStateChange >= Constants.Game.GameModeAbilityDurationMs)
            {
                SetStateInstant(gameState);
                _usedAbilities[gameStateName].Add(playerId);
                _lastAbilityUser = playerId;
                _lastGameStateChange = currentTime;
                return true;
            }

            return false;
        }
    }

    public void ResetState(Guid playerId)
    {
        lock (_gameStateLock)
        {
            if (_lastAbilityUser != playerId)
            {
                return;
            }

            _state = new NormalState();
        }
    }

    public void SetStateInstant(IGameState gameState)
    {
        _state = gameState;
    }
}
