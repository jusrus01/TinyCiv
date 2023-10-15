using TinyCiv.Server.Core.Publishers;
using TinyCiv.Shared.Events.Client.Lobby;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers.Lobby;

public class JoinLobbyHandler : ClientHandler<JoinLobbyClientEvent>
{
    private readonly ISessionService _sessionService;
    private readonly IGameService _gameService;

    public JoinLobbyHandler(ISessionService sessionService, ILogger<JoinLobbyHandler> logger, IGameService gameService, IPublisher publisher) : base(publisher, logger)
    {
        _sessionService = sessionService;
        _gameService = gameService;
    }

    protected override bool IgnoreWhen(JoinLobbyClientEvent @event) =>
        _sessionService.IsStarted() || _sessionService.IsLobbyFull();

    protected override async Task OnHandleAsync(JoinLobbyClientEvent @event)
    {
        var response = _gameService.ConnectPlayer();

        if (response == null) return;

        await NotifyCallerAsync(Constants.Server.SendCreatedPlayer, new JoinLobbyServerEvent(response.Player))
            .ConfigureAwait(false);
        await NotifyCallerAsync(Constants.Server.SendResourcesStatusUpdate, new ResourcesUpdateServerEvent(response.Resources!))
            .ConfigureAwait(false);

        if (response.CanGameStart)
        {
            await NotifyAllAsync(Constants.Server.SendLobbyStateToAll, new LobbyStateServerEvent(response.CanGameStart))
                .ConfigureAwait(false);
        }
    } 
}