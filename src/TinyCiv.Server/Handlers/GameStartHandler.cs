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

    public GameStartHandler(ISessionService sessionService, ILogger<GameStartHandler> logger, IGameService gameService, IPublisher publisher) : base(publisher, gameService, logger)
    {
        _sessionService = sessionService;
    }

    protected override bool IgnoreWhen(StartGameClientEvent @event) =>
        _sessionService.IsStarted() || !_sessionService.CanGameStart();

    protected override async Task OnHandleAsync(StartGameClientEvent @event)
    {
        Map map = GameService.StartGame(@event.MapType);
        await NotifyAllAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(map))
            .ConfigureAwait(false);
        await NotifyAllAsync(Constants.Server.SendGameModeChangeEventToAll, new GameModeChangeServerEvent(GameModeType.Normal));

        map = GameService.InitializeColonists();
        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(map))
            .ConfigureAwait(false);
    }
}