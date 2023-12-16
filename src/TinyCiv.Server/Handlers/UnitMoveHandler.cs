using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Server.Dtos.Units;
using TinyCiv.Server.Enums;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers
{
    public class UnitMoveHandler : ClientHandler<MoveUnitClientEvent>
    {
        private readonly IConnectionIdAccessor _accessor;

        public UnitMoveHandler(ILogger<UnitMoveHandler> logger, IConnectionIdAccessor accessor, IGameService gameService, IPublisher publisher) : base(publisher, gameService, logger)
        {
            _accessor = accessor;    
        }

        protected override Task OnHandleAsync(MoveUnitClientEvent @event)
        {
            async void UnitMoveCallback(UnitMoveResponse response, Map? map)
            {
                switch (response)
                {
                    case UnitMoveResponse.Started:
                        await NotifyCallerAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId.Value, true, _accessor.ConnectionId))
                            .ConfigureAwait(false);
                        break;
                    case UnitMoveResponse.Moved:
                        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(map!, _accessor.ConnectionId))
                            .ConfigureAwait(false);
                        break;
                    case UnitMoveResponse.Stopped:
                        await NotifyCallerAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId.Value, false, _accessor.ConnectionId))
                            .ConfigureAwait(false); 
                        break;
                }
            }

            ServerPosition targetPosition = new() { X = @event.X, Y = @event.Y };
            var request = new MoveUnitRequest(@event.UnitId.Value, targetPosition, UnitMoveCallback);
            GameService.MoveUnit(request);

            return Task.CompletedTask;
        }
    }
}
