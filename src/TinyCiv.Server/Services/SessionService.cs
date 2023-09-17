using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

// Must be singleton
public class SessionService : ISessionService
{
    private readonly object _newPlayerLocker;
    private readonly List<Player> _players;

    private readonly object _mapChangeLocker;
    private Map? _currentMap;
    
    public SessionService()
    {
        _players = new List<Player>();
        
        _newPlayerLocker = new object();
        _mapChangeLocker = new object();
    }
    
    public Player? AddPlayer()
    {
        lock (_newPlayerLocker)
        {
            if (_players.Count >= Constants.Game.MaxPlayerCount)
            {
                return null;
            }
            
            if (!Enum.TryParse<Color>(_players.Count.ToString(), out var playerColor))
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

    public bool IsLobbyFull()
    {
        lock (_newPlayerLocker)
        {
            return _players.Count >= Constants.Game.MaxPlayerCount;
        }
    }

    public bool IsStarted()
    {
        lock (_mapChangeLocker)
        {
            return _currentMap != null;
        }
    }

    public Map InitMap()
    {
        lock (_mapChangeLocker)
        {
            if (_currentMap != null)
            {
                throw new InvalidOperationException("Cannot initialize map twice");
            }

            _currentMap = new Map
            {
                Objects = GenerateObjects()
            };

            return _currentMap;
        }
    }
    
    [Obsolete("Temporary")]
    private List<GameObject> GenerateObjects()
    {
        var objects = new List<GameObject>();
            
        // TODO: read map from file or some other way
        var playerPositions = _players
            .Select(player => new
            {
                OwnerId = player.Id,
                Pos = new Position
                {
                    X = new Random().Next(Constants.Game.WidthSquareCount),
                    Y = new Random().Next(Constants.Game.HeightSquareCount)
                }
            })
            .ToList();
        for (var y = 0; y < Constants.Game.HeightSquareCount; y++)
        {
            for (var x = 0; x < Constants.Game.WidthSquareCount; x++)
            {
                var playerPosition = playerPositions
                    .FirstOrDefault(p => p.Pos.X == x && p.Pos.Y == y);
                if (playerPosition == null)
                {
                    objects.Add(new GameObject
                    {
                        Id = Guid.NewGuid(),
                        Position = new Position
                        {
                            X = x,
                            Y = y
                        },
                        Type = GameObjectType.Empty
                    });
                }
                else
                {
                    objects.Add(new GameObject
                    {
                        Id = Guid.NewGuid(),
                        Position = new Position
                        {
                            X = x,
                            Y = y
                        },
                        OwnerPlayerId = playerPosition.OwnerId,
                        Type = GameObjectType.Warrior
                    }); 
                }
            }
        }

        return objects;
    }
}