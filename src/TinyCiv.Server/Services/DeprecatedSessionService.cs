using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Dto;

namespace TinyCiv.Server.Services;

// Must be singleton

// quick implementation, will need changing
public class DeprecatedSessionService : ISessionService
{
    private readonly object _newPlayerLocker;
    private readonly object _mapChangeLocker;
    
    private readonly List<int> _userIds;
    
    private string _map = string.Empty;

    public DeprecatedSessionService()
    {
        _newPlayerLocker = new object();
        _mapChangeLocker = new object();
        
        _userIds = new List<int>();
    }

    public string StartSession()
    {
        lock (_mapChangeLocker)
        {
            _map = Constants.Game.Map;
            return _map;
        }
    }

    public bool IsValidPlayer(int playerId)
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

    public PlayerDto? AddNewPlayerToGame()
    {
        Random random = new Random();
        var newUser = new PlayerDto(random.Next(1, 4));

        lock (_newPlayerLocker)
        {
            if (_userIds.Count >= Constants.Game.MaxPlayerCount)
            {
                return new PlayerDto(-1);
            }
            
            _userIds.Add(newUser.Id);
        }

        return newUser;
    }
}