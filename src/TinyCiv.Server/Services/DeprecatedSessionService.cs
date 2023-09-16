using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;

namespace TinyCiv.Server.Services;

// Must be singleton

// quick implementation, will need changing
public class DeprecatedSessionService : ISessionService
{
    private readonly object _newPlayerLocker;
    private readonly object _mapChangeLocker;
    
    private readonly List<Guid> _userIds;
    
    private string _map = string.Empty;

    public DeprecatedSessionService()
    {
        _newPlayerLocker = new object();
        _mapChangeLocker = new object();
        
        _userIds = new List<Guid>();
    }

    public string StartSession()
    {
        lock (_mapChangeLocker)
        {
            _map = Constants.Game.Map;
            return _map;
        }
    }

    public bool IsValidPlayer(Guid playerId)
    {
        return _userIds.Contains(playerId);
    }

    public bool IsSessionStarted()
    {
        return !string.IsNullOrEmpty(_map);
    }

    public bool AllPlayersInLobby()
    {
        return _userIds.Count == Constants.Game.MaxPlayerCount;
    }

    public string GetMap()
    {
        lock (_mapChangeLocker)
        {
            return _map;
        }
    }

    public void PlaceUnit(int x, int y)
    {
        // lock (_mapChangeLocker)
        // {
        //     var previous = _map;
        //     try
        //     {
        //         var rows = _map.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        //     }
        //     catch
        //     {
        //         _map = previous;
        //     }
        // }
        throw new NotSupportedException();
    }

    public Guid? AddNewPlayerToGame()
    {
        var newUserId = Guid.NewGuid();
        lock (_newPlayerLocker)
        {
            if (_userIds.Count >= Constants.Game.MaxPlayerCount)
            {
                return null;
            }
            
            _userIds.Add(newUserId);
        }

        return newUserId;
    }
}