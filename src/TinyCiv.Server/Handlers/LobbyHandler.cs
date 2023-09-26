using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers;

public class LobbyHandler : ClientHandler<JoinLobbyClientEvent>
{
    private readonly ISessionService _sessionService;

    public LobbyHandler(ISessionService sessionService, ILogger<LobbyHandler> logger) : base(logger)
    {
        _sessionService = sessionService;
    }

    protected override bool IgnoreWhen(JoinLobbyClientEvent @event) =>
        _sessionService.IsStarted() || _sessionService.IsLobbyFull();

    protected override async Task OnHandleAsync(JoinLobbyClientEvent @event)
    {
        var newPlayer = _sessionService.AddPlayer();
        if (newPlayer == null)
        {
            return;
        }

        await NotifyCallerAsync(Constants.Server.SendCreatedPlayer, new JoinLobbyServerEvent(newPlayer)).ConfigureAwait(false);

        // Allow multiple calls to this, so that new players that join would also get this notification
        if (_sessionService.CanGameStart())
        {
            await NotifyAllAsync(Constants.Server.SendGameStartReadyToAll, new GameStartReadyServerEvent())
                .ConfigureAwait(false);
        }
    } 
}