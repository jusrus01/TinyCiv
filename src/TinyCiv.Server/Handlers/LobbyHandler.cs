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

    public LobbyHandler(ISessionService sessionService)
    {
        _sessionService = sessionService;
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
<<<<<<< HEAD:src/TinyCiv.Server/Handlers/Client/LobbyHandler.cs
            .SendEventAsync(Constants.Server.SendGeneratedId, new JoinLobbyServerEvent(newUserId!))
=======
            .SendEventAsync(Constants.Server.SendCreatedPlayer, new JoinLobbyServerEvent(newPlayer))
>>>>>>> master:src/TinyCiv.Server/Handlers/LobbyHandler.cs
            .ConfigureAwait(false);
        
        if (_sessionService.IsLobbyFull())
        {
            await all
                .SendEventAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(_sessionService.InitMap()))
                .ConfigureAwait(false);
        }
    }
}