using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Entities;
using TinyCiv.Server.Enums;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services
{
    public class MapService : IMapService
    {
        private readonly ISessionService _sessionService;

        private Map? _map;
        private readonly object _mapChangeLocker;
        private List<MovingUnit> _movingUnits;
        private readonly IMapLoader _mapLoader;

        public MapService(IMapLoader mapLoader, ISessionService sessionService)
        {
            _sessionService = sessionService;
            _mapLoader = mapLoader;
            _mapChangeLocker = new object();
            _movingUnits = new List<MovingUnit>();
        }

        public ServerGameObject? CreateUnit(Guid playerId, ServerPosition position)
        {
            var player = _sessionService.GetPlayer(playerId);

            if (player == null)
            {
                return null;
            }

            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    return null;
                }

                int index = _map.Objects!
                    .Select((o, i) => new { Value = o, Index = i })
                    .FirstOrDefault(o => o.Value.Position!.X == position.X && o.Value.Position.Y == position.Y)!
                    .Index;

                if (_map.Objects![index].Type != GameObjectType.Empty)
                {
                    return null;
                }

                var unit = new ServerGameObject
                {
                    Id = Guid.NewGuid(),
                    OwnerPlayerId = playerId,
                    Position = position,
                    Type = GameObjectType.Warrior,
                    Color = player.Color
                };

                _map.Objects![index] = unit;

                return unit;
            }
        }

        private bool MoveUnit(Guid unitId, ServerPosition position)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    return false;
                }

                var occupiedUnit = _map.Objects!.Single(o => o.Position!.X == position.X && o.Position.Y == position.Y);

                if (occupiedUnit.Type != GameObjectType.Empty)
                {
                    return false;
                }

                var unit = GetUnit(unitId);

                if (unit == null)
                {
                    return false;
                }

                occupiedUnit.Position = unit.Position;
                unit.Position = position;
                return true;
            }
        }

        public async Task MoveUnitAsync(Guid unitId, ServerPosition position, Action<UnitMoveResponse> unitMoveCallback)
        {
            var unit = GetUnit(unitId);

            if (unit == null || !IsValidPosition(position))
            {
                return;
            }

            var movingUnitEntry = _movingUnits.SingleOrDefault(u => u.UnitId == unitId);

            if (movingUnitEntry != null)
            {
                movingUnitEntry.CancellationTokenSource.Cancel();
                RemoveMovingUnit(unitId);
            }

            unitMoveCallback?.Invoke(UnitMoveResponse.Started);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            AddMovingUnit(unitId, position, cancellationTokenSource);

            await Task.Run(async () =>
            {
                while (unit.Position!.X != position.X || unit.Position.Y != position.Y)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    await Task.Delay(Constants.Game.MovementSpeedMs);

                    int diffX = Math.Clamp(position.X - unit.Position.X, -1, 1);
                    int diffY = Math.Clamp(position.Y - unit.Position.Y, -1, 1);
                    var nextPosition = new ServerPosition { X = unit.Position.X + diffX, Y = unit.Position.Y + diffY };

                    bool didUnitMove = MoveUnit(unitId, nextPosition);

                    if (didUnitMove == false)
                    {
                        // Need pathfinding algorithm, because now unit stops moving when collided with another unit
                        RemoveMovingUnit(unitId);
                        unitMoveCallback?.Invoke(UnitMoveResponse.Stopped);
                        return;
                    }

                    unitMoveCallback?.Invoke(UnitMoveResponse.Moved);
                }

                unitMoveCallback?.Invoke(UnitMoveResponse.Stopped);
            }, cancellationToken);
        }

        private bool AddMovingUnit(Guid unitId, ServerPosition position, CancellationTokenSource cancellationTokenSource)
        {
            lock (_mapChangeLocker)
            {
                _movingUnits.Add(new MovingUnit(unitId, position, cancellationTokenSource));
                return true;
            }
        }

        private void RemoveMovingUnit(Guid unitId)
        {
            lock (_mapChangeLocker)
            {
                var movingUnitEntry = _movingUnits.Single(u => u.UnitId == unitId);
                _movingUnits.Remove(movingUnitEntry);
            }
        }

        public ServerGameObject? GetUnit(Guid unitId)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    return null;
                }

                var unit = _map.Objects!.Single(o => o.Id == unitId);

                return unit;
            }
        }

        public ServerGameObject? GetUnit(ServerPosition position)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    return null;
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

        public Map? Initialize(MapType mapType)
        {
            lock (_mapChangeLocker)
            {
                if (_map != null)
                {
                    return null;
                }

                _map = new Map
                {
                    Objects = _mapLoader.Load(mapType)
                };

                return _map;
            }
        }

        private bool IsValidPosition(ServerPosition position)
        {
            var occupiedUnit = GetUnit(position);

            if (occupiedUnit != null && occupiedUnit.Type != GameObjectType.Empty)
            {
                return false;
            }

            if (position.X >= Constants.Game.WidthSquareCount || position.X < 0 ||
                position.Y >= Constants.Game.HeightSquareCount || position.Y < 0)
            {
                return false;
            }

            return true;
        }
    }
}
