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

    private bool _isGameStarted;

    public SessionService()
    {
        _players = new List<Player>();
        _playerLocker = new object();
    }

    public Player? AddPlayer(string connectionId)
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
                Color = playerColor,
                ConnectionId = connectionId
            };

            _players.Add(player);

            return player;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    public Player GetPlayer(Guid playerId)
    {
        // Need this check, to control the context in which this is used
        // after game is started there should be no changes happening to players e.g.
        // no new ones should be added and none should be removed, otherwise we would
        // need to lock _players collection
        if (!_isGameStarted)
        {
            throw new InvalidOperationException();
        }
        
        return _players.Single(p => p.Id == playerId);
    }

    public void RemovePlayerByConnectionId(string connectionId)
    {
        if (_isGameStarted)
        {
            throw new InvalidOperationException("Cannot remove player after game has started");
        }
        
        lock (_playerLocker)
        {
            var playerToRemove = _players.SingleOrDefault(player => player.ConnectionId == connectionId);
            if (playerToRemove == null)
            {
                return;
            }

            _players.Remove(playerToRemove);
        }
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