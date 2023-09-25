using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Enums;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers
{
    public class UnitMoveHandler : ClientHandler<MoveUnitClientEvent>
    {
        private readonly IMapService _mapService;

        public UnitMoveHandler(IMapService mapService, ILogger<UnitMoveHandler> logger) : base(logger)
        {
            _mapService = mapService;
        }

        protected override Task OnHandleAsync(IClientProxy caller, IClientProxy all, MoveUnitClientEvent @event)
        {
            async void unitMoveCallback(UnitMoveResponse response)
            {
                switch (response)
                {
                    case UnitMoveResponse.Started:
                        await caller
                            .SendEventAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId, true))
                            .ConfigureAwait(false);
                        break;
                    case UnitMoveResponse.Moved:
                        await all
                            .SendEventAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
                            .ConfigureAwait(false);
                        break;
                    case UnitMoveResponse.Stopped:
                        await caller
                            .SendEventAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId, false))
                            .ConfigureAwait(false); 
                        break;
                }
            }

            ServerPosition targetPosition = new() { X = @event.X, Y = @event.Y };
            _mapService.MoveUnitAsync(@event.UnitId, targetPosition, unitMoveCallback);

            return Task.CompletedTask;
        }
    }
}
