using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers;

public class GameStartHandler : ClientHandler<StartGameClientEvent>
{
    private readonly IMapService _mapService;
    private readonly ISessionService _sessionService;

    public GameStartHandler(ISessionService sessionService, IMapService mapService)
    {
        _sessionService = sessionService;
        _mapService = mapService;
    }

    protected override bool IgnoreWhen(StartGameClientEvent @event) =>
        _sessionService.IsStarted();

    protected override async Task OnHandleAsync(IClientProxy caller, IClientProxy all, StartGameClientEvent @event)
    {
        _sessionService.StartGame();
        var map = _mapService.Initialize() ?? throw new InvalidOperationException("Something went wrong, unable to initialize map");
    
        await all
            .SendEventAsync(Constants.Server.SendGameStartToAll, new GameStartServerEvent(map))
            .ConfigureAwait(false);
    }
}