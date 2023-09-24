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
    private readonly IMapService _mapService;

    public LobbyHandler(ISessionService sessionService, IMapService mapService)
    {
        _sessionService = sessionService;
        _mapService = mapService;
    }

    // TODO: consider throwing to force client to correctly send events
    // TODO: consider making game objects as classes that are wrapped in record
    protected override bool IgnoreWhen(JoinLobbyClientEvent @event)
    {
        return _sessionService.IsStarted() || _sessionService.IsLobbyFull();
    }

    protected override async Task OnHandleAsync(IClientProxy caller, IClientProxy all, JoinLobbyClientEvent @event)
    {
        var newPlayer = _sessionService.AddPlayer();

        if (newPlayer == null)
        {
            return;
        }
        
        await caller
            .SendEventAsync(Constants.Server.SendCreatedPlayer, new JoinLobbyServerEvent(newPlayer))
            .ConfigureAwait(false);
        
        if (_sessionService.IsLobbyFull())
        {
            _sessionService.StartGame();

            var map = _mapService.Initialize();

            if (map == null)
            {
                return;
            }

            await all
                .SendEventAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(map))
                .ConfigureAwait(false);
        }
    } 
}