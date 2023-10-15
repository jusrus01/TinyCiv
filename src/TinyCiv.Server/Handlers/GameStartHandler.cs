using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class GameStartHandler : ClientHandler<StartGameClientEvent>
{
    private readonly ISessionService _sessionService;
    private readonly IGameService _gameService;

    public GameStartHandler(ISessionService sessionService, ILogger<GameStartHandler> logger, IGameService gameService, IPublisher publisher) : base(publisher, logger)
    {
        _sessionService = sessionService;
        _gameService = gameService;
    }

    protected override bool IgnoreWhen(StartGameClientEvent @event) =>
        _sessionService.IsStarted() || !_sessionService.CanGameStart();

    protected override Task OnHandleAsync(StartGameClientEvent @event)
    {
        Map map = _gameService.StartGame(@event.MapType);
        return NotifyAllAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(map));
    }
}