using TinyCiv.Shared.Events.Client.Lobby;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers.Lobby;

public class LeaveLobbyHandler : ClientHandler<LeaveLobbyClientEvent>
{
    private readonly ISessionService _sessionService;
    private readonly IGameService _gameService;

    public LeaveLobbyHandler(
        ISessionService sessionService,
        ILogger<IClientHandler> logger,
        IGameService gameService,
        IPublisher publisher)
        :
        base(publisher, logger)
    {
        _sessionService = sessionService;
        _gameService = gameService;
    }

    // Do not allow leaving once the game starts.
    // On cases when 2 players are playing and one of them disconnects, then force first player to finish the game.
    protected override bool IgnoreWhen(LeaveLobbyClientEvent @event) => _sessionService.IsStarted();

    protected override Task OnHandleAsync(LeaveLobbyClientEvent @event)
    {
        var response = _gameService.DisconnectPlayer();

        if (!response.CanGameStart)
        {
            return NotifyAllAsync(Constants.Server.SendLobbyStateToAll, new LobbyStateServerEvent(response.CanGameStart));
        }
        
        return Task.CompletedTask;
    }
}