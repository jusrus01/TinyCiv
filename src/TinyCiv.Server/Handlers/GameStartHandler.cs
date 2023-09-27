using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using Constants = TinyCiv.Shared.Constants;

namespace TinyCiv.Server.Handlers;

public class GameStartHandler : ClientHandler<StartGameClientEvent>
{
    private readonly IMapService _mapService;
    private readonly ISessionService _sessionService;

    public GameStartHandler(ISessionService sessionService, IMapService mapService, ILogger<GameStartHandler> logger) : base(logger)
    {
        _sessionService = sessionService;
        _mapService = mapService;
    }

    protected override bool IgnoreWhen(StartGameClientEvent @event) =>
        _sessionService.IsStarted() || !_sessionService.CanGameStart();

    protected override Task OnHandleAsync(StartGameClientEvent @event)
    {
        _sessionService.StartGame();
        var map = _mapService.Initialize(@event.MapType) ?? throw new InvalidOperationException("Something went wrong, unable to initialize map");

        return NotifyAllAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(map));
    }
}