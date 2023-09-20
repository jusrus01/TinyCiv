using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers
{
    public class UnitMoveHandler : ClientHandler<MoveUnitClientEvent>
    {
        private readonly IMapService _mapService;

        private readonly object _movementLocker = new();

        public UnitMoveHandler(IMapService mapService)
        {
            _mapService = mapService;
        }

        protected override async Task OnHandleAsync(IClientProxy caller, IClientProxy all, MoveUnitClientEvent @event)
        {
            var unitMovementTuple = (@event.UnitId, new ServerPosition { X = @event.X, Y = @event.Y });

            lock (_movementLocker)
            {
                // Yes, it is being checked 2 times :| Just put the validation into another function?
                if (IgnoreWhen(@event))
                {
                    return;
                }

                _mapService.MovingUnits.Add(unitMovementTuple);
            }

            await caller
                .SendEventAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId, true))
                .ConfigureAwait(false);

            var unit = _mapService.GetUnit(@event.UnitId);

            _ = Task.Run(async () =>
            {
                while (unit.Position!.X != @event.X || unit.Position.Y != @event.Y)
                {
                    await Task.Delay(Constants.Game.MovementSpeedMs);

                    int diffX = Math.Clamp(@event.X - unit.Position.X, -1, 1);
                    int diffY = Math.Clamp(@event.Y - unit.Position.Y, -1, 1);

                    try
                    {
                        _mapService.MoveUnit(@event.UnitId, new ServerPosition { X = unit.Position.X + diffX, Y = unit.Position.Y + diffY });
                    }
                    catch
                    {
                        _mapService.MovingUnits.Remove(unitMovementTuple);

                        // Need pathfinding algorithm, because now unit stops moving when collided with another unit
                        await caller
                            .SendEventAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId, false))
                            .ConfigureAwait(false);

                        return;
                    }

                    await all
                        .SendEventAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
                        .ConfigureAwait(false);
                }

                _mapService.MovingUnits.Remove(unitMovementTuple);

                // Should everyone know that unit stopped moving, or just the caller? Will we have a visual for moving unit?
                await caller
                    .SendEventAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId, false))
                    .ConfigureAwait(false);

            });
        }

        protected override bool IgnoreWhen(MoveUnitClientEvent @event)
        {
            if (_mapService.MovingUnits.Any(u => u.Item1 == @event.UnitId))
            {
                return true;
            }

            if (_mapService.MovingUnits.Any(u => u.Item2 == new ServerPosition { X = @event.X, Y = @event.Y }))
            {
                return true;
            }

            var occupiedUnit = _mapService.GetUnit(new ServerPosition { X = @event.X, Y = @event.Y });

            if (occupiedUnit.Type != GameObjectType.Empty)
            {
                return true;
            }

            if (@event.X >= Constants.Game.WidthSquareCount || @event.X < 0 ||
                @event.Y >= Constants.Game.HeightSquareCount || @event.Y < 0)
            {
                return true;
            }

            return false;
        }
    }
}
