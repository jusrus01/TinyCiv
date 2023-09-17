using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services
{
    public class MapService : IMapService
    {
        private Map? _map;
        private readonly object _mapChangeLocker;

        public MapService()
        {
            _mapChangeLocker = new object();
        }

        public void AddUnit(Guid playerId, Position position)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    throw new InvalidOperationException("Map is not initialized");
                }

                int index = _map.Objects!
                    .Select((o, i) => new { Value = o, Index = i })
                    .FirstOrDefault(o => o.Value.Position!.X == position.X && o.Value.Position.Y == position.Y)!
                    .Index;

                if (_map.Objects![index].Type != GameObjectType.Empty)
                {
                    throw new InvalidOperationException("Another GameObject is blocking specified position");
                }

                var unit = new GameObject
                {
                    Id = Guid.NewGuid(),
                    OwnerPlayerId = playerId,
                    Position = position,
                    Type = GameObjectType.Warrior
                };

                _map.Objects![index] = unit;
            }
        }

        public Map? GetMap()
        {
            lock (_mapChangeLocker)
            {
                return _map;
            }
        }

        public Map Initialize()
        {
            lock (_mapChangeLocker)
            {
                if (_map != null)
                {
                    throw new InvalidOperationException("Cannot initialize map twice");
                }

                _map = new Map
                {
                    Objects = GenerateObjects()
                };

                return _map;
            }
        }

        [Obsolete("Temporary")]
        private List<GameObject> GenerateObjects()
        {
            var objects = new List<GameObject>();

            // TODO: read map from file or some other way
            for (var y = 0; y < Constants.Game.HeightSquareCount; y++)
            {
                for (var x = 0; x < Constants.Game.WidthSquareCount; x++)
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
            }

            return objects;
        }
    }
}
