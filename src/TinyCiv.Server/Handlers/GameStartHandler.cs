using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;
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

        var players = _sessionService.GetPlayers();

        foreach (var player in players)
        {
            var random = new Random();
            int x = random.Next(0, Constants.Game.WidthSquareCount);
            int y = random.Next(0, Constants.Game.HeightSquareCount);
            var position = new ServerPosition { X = x, Y = y };

            while (_mapService.IsInRange(position, Constants.Game.TownSpaceFromTown, GameObjectType.Colonist))
            {
                x = random.Next(0, Constants.Game.WidthSquareCount);
                y = random.Next(0, Constants.Game.HeightSquareCount);
                position = new ServerPosition { X = x, Y = y };
            }

            _mapService.CreateUnit(player.Id, position, GameObjectType.Colonist);
        }

        return NotifyAllAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(map));
    }
}