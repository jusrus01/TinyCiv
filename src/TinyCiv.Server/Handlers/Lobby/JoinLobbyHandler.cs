using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client.Lobby;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers.Lobby;

public class JoinLobbyHandler : ClientHandler<JoinLobbyClientEvent>
{
    private readonly ISessionService _sessionService;
    private readonly IConnectionIdAccessor _accessor;
    private readonly IResourceService _resourceService;

    public JoinLobbyHandler(ISessionService sessionService, IConnectionIdAccessor accessor, ILogger<JoinLobbyHandler> logger, IResourceService resourceService) : base(logger)
    {
        _sessionService = sessionService;
        _accessor = accessor;
        _resourceService = resourceService;
    }

    protected override bool IgnoreWhen(JoinLobbyClientEvent @event) =>
        _sessionService.IsStarted() || _sessionService.IsLobbyFull();

    protected override async Task OnHandleAsync(JoinLobbyClientEvent @event)
    {
        var newPlayer = _sessionService.AddPlayer(_accessor.ConnectionId);
        if (newPlayer == null)
        {
            return;
        }

        _resourceService.InitializeResources(newPlayer.Id);

        await NotifyCallerAsync(Constants.Server.SendCreatedPlayer, new JoinLobbyServerEvent(newPlayer)).ConfigureAwait(false);

        var canGameStart = _sessionService.CanGameStart();
        if (canGameStart)
        {
            await NotifyAllAsync(Constants.Server.SendLobbyStateToAll, new LobbyStateServerEvent(canGameStart))
                .ConfigureAwait(false);
        }
    } 
}