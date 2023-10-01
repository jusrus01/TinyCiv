using System.Runtime.ConstrainedExecution;
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

        private bool MoveUnit(Guid unitId, ServerPosition target)
        {
            lock (_mapChangeLocker)
            {
                var targetTile = _map.Objects!.Single(o => o.Position.Equals(target));
                var unit = GetUnit(unitId);
                targetTile.Position = unit.Position;
                unit.Position = target;
                return true;
            }
        }

        public async Task MoveUnitAsync(Guid unitId, ServerPosition targetPos, Action<UnitMoveResponse> unitMoveCallback)
        {
            var unit = GetUnit(unitId);

            if (!IsValidTargetPosition(targetPos))
            {
                return;
            }

            CancelExistingMovement(unitId);

            unitMoveCallback?.Invoke(UnitMoveResponse.Started);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            AddMovingUnit(unitId, targetPos, cancellationTokenSource);

            var path = AStar.FindPath(_map, unit.Position, targetPos);

            await Task.Run(async () =>
            {
                foreach (var pathPos in path)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var nextTile = GetTileAtPosition(pathPos);

                    // units start a combat
                    if (pathPos == path[path.Count - 1] && IsTileOccupied(nextTile))
                    {
                        unit.OpponentId = nextTile.Id;
                        nextTile.OpponentId = unit.Id;
                        unitMoveCallback?.Invoke(UnitMoveResponse.Moved);
                        return;
                    }

                    if (IsTileOccupied(nextTile))
                    {
                        // Need to recompute the path because the current path is blocked
                        path = AStar.FindPath(_map, unit.Position, targetPos);
                        if (path.Count == 0)
                        {
                            // No valid path found, stop moving
                            StopUnitMovement(unitId, unitMoveCallback);
                            return;
                        }
                    }

                    if (pathPos == path[0])
                    {
                        var enemy = GetUnit(unit.OpponentId);
                        unit.OpponentId = null;
                        if (enemy != null)
                        {
                            enemy.OpponentId = null;
                        }
                        Console.WriteLine("RAN FROM A BATTLE");
                        unitMoveCallback?.Invoke(UnitMoveResponse.Moved);
                    }

                    // unit runs from a combat
                    if (pathPos != path[0])
                    {                      
                        await Task.Delay(Constants.Game.MovementSpeedMs);
                    }                    

                    MoveUnit(unitId, pathPos);

                    unitMoveCallback?.Invoke(UnitMoveResponse.Moved);
                }

                StopUnitMovement(unitId, unitMoveCallback);
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

        public ServerGameObject? GetUnit(Guid? unitId)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    return null;
                }

                if (unitId == null)
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

        private bool IsValidTargetPosition(ServerPosition position)
        {
            var targetUnit = GetUnit(position);

            if (targetUnit == null)
            {
                return false;
            }

            if (IsObstacle(targetUnit))
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

        private bool IsTileOccupied(ServerGameObject obj)
        {
            return obj.Type != GameObjectType.Empty;
        }

        private bool IsObstacle(ServerGameObject o) => new[] {
            GameObjectType.StaticMountain,
            GameObjectType.StaticWater,
        }.Contains(o.Type);

        private void CancelExistingMovement(Guid unitId)
        {
            var movingUnitEntry = _movingUnits.SingleOrDefault(u => u.UnitId == unitId);
            if (movingUnitEntry != null)
            {
                movingUnitEntry.CancellationTokenSource.Cancel();
                RemoveMovingUnit(unitId);
            }
        }

        private void StopUnitMovement(Guid unitId, Action<UnitMoveResponse> unitMoveCallback)
        {
            RemoveMovingUnit(unitId);
            unitMoveCallback?.Invoke(UnitMoveResponse.Stopped);
        }

        private ServerGameObject GetTileAtPosition(ServerPosition position)
        {
            return _map.Objects!.Single(o => o.Position.Equals(position));
        }
    }
}
