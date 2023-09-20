using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services
{
    public class MapService : IMapService
    {
        private Map? _map;
        private readonly object _mapChangeLocker;

        // Temporary?
        public List<(Guid, ServerPosition)> MovingUnits { get; set; }

        public MapService()
        {
            _mapChangeLocker = new object();
            MovingUnits = new List<(Guid, ServerPosition)>();
        }

        public ServerGameObject AddUnit(Guid playerId, ServerPosition position)
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

                var unit = new ServerGameObject
                {
                    Id = Guid.NewGuid(),
                    OwnerPlayerId = playerId,
                    Position = position,
                    Type = GameObjectType.Warrior
                };

                _map.Objects![index] = unit;

                return unit;
            }
        }

        public void MoveUnit(Guid unitId, ServerPosition position)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    throw new InvalidOperationException("Map is not initialized");
                }

                var occupiedUnit = _map.Objects!.Single(o => o.Position!.X == position.X && o.Position.Y == position.Y);

                if (occupiedUnit.Type != GameObjectType.Empty)
                {
                    throw new InvalidOperationException("Specified space is occupied");
                }

                var unit = _map.Objects!.Single(o => o.Id == unitId);

                occupiedUnit.Position = unit.Position;
                unit.Position = position;
            }
        }

        public ServerGameObject GetUnit(Guid unitId)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    throw new InvalidOperationException("Map is not initialized");
                }

                var unit = _map.Objects!.Single(o => o.Id == unitId);

                return unit;
            }
        }

        public ServerGameObject GetUnit(ServerPosition position)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    throw new InvalidOperationException("Map is not initialized");
                }

                var unit = _map.Objects!.Single(o => o.Position!.X == position.X && o.Position!.Y == position.Y);

                return unit;
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
        private List<ServerGameObject> GenerateObjects()
        {
            var objects = new List<ServerGameObject>();

            // TODO: read map from file or some other way
            for (var y = 0; y < Constants.Game.HeightSquareCount; y++)
            {
                for (var x = 0; x < Constants.Game.WidthSquareCount; x++)
                {
                    objects.Add(new ServerGameObject
                    {
                        Id = Guid.NewGuid(),
                        Position = new ServerPosition
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
