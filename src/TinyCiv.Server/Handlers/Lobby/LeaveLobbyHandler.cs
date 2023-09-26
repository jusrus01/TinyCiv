using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client.Lobby;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers.Lobby;

public class LeaveLobbyHandler : ClientHandler<LeaveLobbyClientEvent>
{
    private readonly ISessionService _sessionService;
    private readonly IConnectionIdAccessor _accessor;

    public LeaveLobbyHandler(
        ISessionService sessionService,
        IConnectionIdAccessor accessor,
        ILogger<IClientHandler> logger)
        :
        base(logger)
    {
        _accessor = accessor;
        _sessionService = sessionService;
    }

    // Do not allow leaving once the game starts.
    // On cases when 2 players are playing and one of them disconnects, then force first player to finish the game.
    protected override bool IgnoreWhen(LeaveLobbyClientEvent @event) => _sessionService.IsStarted();

    protected override Task OnHandleAsync(LeaveLobbyClientEvent @event)
    {
        _sessionService.RemovePlayerByConnectionId(_accessor.ConnectionId);

        var canGameStart = _sessionService.CanGameStart();
        // Assume other handler sent previously that game could start
        if (!canGameStart)
        {
            return NotifyAllAsync(Constants.Server.SendLobbyStateToAll, new LobbyStateServerEvent(canGameStart));
        }
        
        return Task.CompletedTask;
    }
}