using System.Diagnostics.CodeAnalysis;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

// Must be singleton
public class SessionService : ISessionService
{
    private readonly object _playerLocker;
    private readonly List<Player> _players;

    private bool _isGameStarted = false;

    public SessionService()
    {
        _players = new List<Player>();
        _playerLocker = new object();
    }

    public Player? AddPlayer()
    {
        lock (_playerLocker)
        {
            if (_players.Count >= Constants.Game.MaxPlayerCount)
            {
                return null;
            }

            if (!Enum.TryParse<TeamColor>(_players.Count.ToString(), out var playerColor))
            {
                return null;
            }

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Color = playerColor
            };

            _players.Add(player);

            return player;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    public Player GetPlayer(Guid playerId)
    {
        return _players.Single(p => p.Id == playerId);
    }

    public void StartGame()
    {
        _isGameStarted = true;
    }

    public bool IsLobbyFull()
    {
        lock (_playerLocker)
        {
            return _players.Count >= Constants.Game.MaxPlayerCount;
        }
    }

    public bool IsStarted()
    {
        return _isGameStarted;
    }

    public bool CanGameStart()
    {
        // Locking in case later we will support "leaving" from the session
        lock (_playerLocker)
        {
            return _players.Count >= Constants.Game.MinPlayerCount;
        }
    }
}