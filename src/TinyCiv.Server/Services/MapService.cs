using Microsoft.AspNetCore.Components.Web;
using System.Security.Cryptography;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Game.Buildings;
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

        public ServerGameObject? CreateBuilding(Guid playerId, ServerPosition position, IBuilding building)
        {
            lock (_mapChangeLocker)
            {
                if (_map == null)
                {
                    return null;
                }

                bool isTileCorrect = _map.Objects!
                        .Where(o => o.Position! == position)
                        .Where(o => o.Type == building.TileType)
                        .Any();

                var player = _sessionService.GetPlayer(playerId);

                bool isTownInRange = IsTownInRange(position, Constants.Game.BuildingSpaceFromTown);

                bool doesEnumExist = Enum.TryParse<GameObjectType>(Enum.GetName(building.BuildingType), out var gameObjectType);

                if (isTileCorrect = false || player == null || isTownInRange || doesEnumExist == false)
                {
                    return null;
                }

                var mapBuilding = new ServerGameObject
                {
                    Id = Guid.NewGuid(),
                    OwnerPlayerId = playerId,
                    Position = position,
                    Type = gameObjectType,
                    Color = player!.Color
                };

                int tileIndex = _map.Objects!
                    .Select((o, i) => new { Value = o, Index = i })
                    .FirstOrDefault(o => o.Value.Position!.X == position.X && o.Value.Position.Y == position.Y)!
                    .Index;

                _map.Objects![tileIndex] = mapBuilding;

                return mapBuilding;
            }
        }

        private bool IsTownInRange(ServerPosition position, int range)
        {
            for (int x = position.X - range; x < position.X + range; x++)
            {
                for (int y = position.Y - range; y < position.Y + range; y++)
                {
                    bool isTown = _map!.Objects!
                        .Where(o => o.Position!.X == x && o.Position.Y == y)
                        .Where(o => o.Type == GameObjectType.Town)
                        .Any();

                    if (isTown)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public ServerGameObject? CreateUnit(Guid playerId, ServerPosition position, GameObjectType type)
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
                    Type = type,
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
                if (unit == null)
                {
                    return false;
                }
                
                targetTile.Position = unit.Position;
                unit.Position = target;
                return true;
            }
        }

        public bool PlaceCity(Guid playerId)
        {
            var colonistObject = _map!.Objects!
                .Select((o, i) => new { Value = o, Index = i })
                .Where(o => o.Value.Type == GameObjectType.Colonist)
                .Where(o => o.Value.OwnerPlayerId == playerId)
                .FirstOrDefault();

            if (colonistObject == null)
            {
                return false;
            }

            if (IsTownInRange(colonistObject.Value.Position!, Constants.Game.TownSpaceFromTown))
            {
                return false;
            }

            _map.Objects![colonistObject.Index] = new ServerGameObject
            {
                OwnerPlayerId = playerId,
                Id = colonistObject.Value.Id,
                Position = colonistObject.Value.Position,
                Type = GameObjectType.Town,
                Color = colonistObject.Value.Color
            };

            return true;
        }

        public bool IsTileFree(ServerPosition position)
        {
            var tile = _map!.Objects!
                .Where(o => o.Position!.X == position.X && o.Position.Y == position.Y)
                .First();

            if (tile.Type == GameObjectType.Empty || tile.Type == GameObjectType.StaticMountain)
            {
                return true;
            }

            return false;
        }

        public bool IsCityOwner(Guid playerId)
        {
            return _map!.Objects!
                .Where(o => o.Type == GameObjectType.Town)
                .Where(o => o.OwnerPlayerId == playerId)
                .Any();
        }

        public void ReplaceWithEmpty(Guid id)
        {
            lock (_mapChangeLocker)
            {
                var objectToReplace = _map?.Objects?.SingleOrDefault(obj => obj.Id == id);
                if (objectToReplace == null)
                {
                    return;
                }

                if (objectToReplace.OpponentId != null)
                {
                    var attacker = GetUnit(objectToReplace.OpponentId);
                    if (attacker != null)
                    {
                        attacker.OpponentId = null;
                    }
                }
                
                var emptyGameObject = new ServerGameObject
                {
                    Id = Guid.NewGuid(),
                    Position = new ServerPosition
                    {
                        X = objectToReplace.Position?.X ?? throw new Exception(),
                        Y = objectToReplace.Position?.Y ?? throw new Exception()
                    },
                    Type = GameObjectType.Empty
                };
                
                _map?.Objects?.Remove(objectToReplace);
                _map?.Objects?.Add(emptyGameObject);
            }
        }

        public async Task MoveUnitAsync(Guid unitId, ServerPosition targetPos, Action<UnitMoveResponse> unitMoveCallback)
        {
            var unit = GetUnit(unitId);

            if (!IsValidTargetPosition(unit, targetPos))
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
                DropAgro(unit, unitMoveCallback);

                foreach (var pathPos in path)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var nextTile = GetTileAtPosition(pathPos);

                    if (pathPos == path.Last() && IsTileOccupied(nextTile))
                    {
                        StartAgro(unit, nextTile, unitMoveCallback);
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

                    // give a quicker first move
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

        private void StartAgro(ServerGameObject unitAttacker, ServerGameObject unitUnderAttack, Action<UnitMoveResponse> unitMoveCallback)
        {
            if (unitAttacker.OwnerPlayerId == unitUnderAttack.OwnerPlayerId)
            {
                return;
            }

            unitAttacker.OpponentId = unitUnderAttack.Id;
            if (unitUnderAttack.OpponentId == null)
            {
                unitUnderAttack.OpponentId = unitAttacker.Id;
            }
            unitMoveCallback?.Invoke(UnitMoveResponse.Moved);
        }

        private void DropAgro(ServerGameObject unit, Action<UnitMoveResponse> unitMoveCallback)
        {
            List<ServerGameObject> enemies = _map.Objects.FindAll(o => o.OpponentId == unit.Id);
            unit.OpponentId = null;
            if (enemies != null)
            {
                enemies.ForEach(e => e.OpponentId = null);
            }
            unitMoveCallback?.Invoke(UnitMoveResponse.Moved);
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

                var unit = _map.Objects!.SingleOrDefault(o => o.Id == unitId);

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

                var unit = _map.Objects!.SingleOrDefault(o => o.Position!.X == position.X && o.Position!.Y == position.Y);

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

        private bool IsValidTargetPosition(ServerGameObject? unit, ServerPosition position)
        {
            var targetUnit = GetUnit(position);

            if (unit == null || targetUnit == null || IsObstacle(targetUnit) || unit.OwnerPlayerId == targetUnit.OwnerPlayerId)
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
