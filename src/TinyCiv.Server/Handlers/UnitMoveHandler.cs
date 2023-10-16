﻿using TinyCiv.Server.Core.Publishers;
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
        private readonly IGameService _gameService;

        public UnitMoveHandler(ILogger<UnitMoveHandler> logger, IGameService gameService, IPublisher publisher) : base(publisher, logger)
        {
            _gameService = gameService;
        }

        protected override Task OnHandleAsync(MoveUnitClientEvent @event)
        {
            async void unitMoveCallback(UnitMoveResponse response, Map? map)
            {
                switch (response)
                {
                    case UnitMoveResponse.Started:
                        await NotifyCallerAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId, true))
                            .ConfigureAwait(false);
                        break;
                    case UnitMoveResponse.Moved:
                        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(map!))
                            .ConfigureAwait(false);
                        break;
                    case UnitMoveResponse.Stopped:
                        await NotifyCallerAsync(Constants.Server.SendUnitStatusUpdate, new UnitStatusUpdateServerEvent(@event.UnitId, false))
                            .ConfigureAwait(false); 
                        break;
                }
            }

            ServerPosition targetPosition = new() { X = @event.X, Y = @event.Y };
            var request = new MoveUnitRequest(@event.UnitId, targetPosition, unitMoveCallback);
            _gameService.MoveUnit(request);

            return Task.CompletedTask;
        }
    }
}
