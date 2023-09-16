using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers.Client;

public class LobbyHandler : ClientHandler<JoinLobbyClientEvent>
{
    private readonly ISessionService _sessionService;

    public LobbyHandler(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    protected override async Task OnHandleAsync(IClientProxy caller, IClientProxy all, JoinLobbyClientEvent @event)
    {
        if (_sessionService.IsSessionStarted())
        {
            return;
        }
        
        var newUserId = _sessionService.AddNewPlayerToGame();
        await caller
            .SendEventAsync(Constants.Server.SendGeneratedId, new JoinLobbyServerEvent(newUserId!.Value))
            .ConfigureAwait(false);
        
        if (_sessionService.AllPlayersInLobby())
        {
            await all
                .SendEventAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(_sessionService.StartSession()))
                .ConfigureAwait(false);
        }
    }
}