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
    private readonly IConnectionIdAccessor _accessor;

    public LeaveLobbyHandler(
        ISessionService sessionService,
        ILogger<IClientHandler> logger,
        IGameService gameService,
        IConnectionIdAccessor accessor,
        IPublisher publisher)
        :
        base(publisher, gameService, logger)
    {
        _sessionService = sessionService;
        _accessor = accessor;
    }

    // Do not allow leaving once the game starts.
    // On cases when 2 players are playing and one of them disconnects, then force first player to finish the game.
    // protected override bool IgnoreWhen(LeaveLobbyClientEvent @event) => _sessionService.IsStarted();

    protected override Task OnHandleAsync(LeaveLobbyClientEvent @event)
    {
        var response = GameService.DisconnectPlayer();

        if (!response.CanGameStart)
        {
            return NotifyAllAsync(Constants.Server.SendLobbyStateToAll, new LobbyStateServerEvent(response.CanGameStart, _accessor.ConnectionId));
        }
        
        return Task.CompletedTask;
    }
}