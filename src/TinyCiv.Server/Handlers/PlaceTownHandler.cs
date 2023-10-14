using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class PlaceTownHandler : ClientHandler<PlaceTownClientEvent>
{
    private readonly IGameService _gameService;

    public PlaceTownHandler(ILogger<IClientHandler> logger, IGameService gameService) : base(logger)
    {
        _gameService = gameService;
    }

    protected override async Task OnHandleAsync(PlaceTownClientEvent @event)
    {
        var map = _gameService.PlaceTown(@event.PlayerId);

        if (map == null) return;

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(map))
            .ConfigureAwait(false);
    }
}
