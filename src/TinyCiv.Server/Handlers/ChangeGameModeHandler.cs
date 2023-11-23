using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class ChangeGameModeHandler : ClientHandler<ChangeGameModeClientEvent>
{
    public ChangeGameModeHandler(
        IPublisher publisher,
        IGameService gameService,
        ILogger<IClientHandler> logger) : base(publisher, gameService, logger)
    {
    }

    protected override async Task OnHandleAsync(ChangeGameModeClientEvent @event)
    {
        var response = GameService.SetGameMode(@event.GameModeType);

        if (response == false)
        {
            return;
        } 

        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new GameModeChangeServerEvent(@event.GameModeType))
            .ConfigureAwait(false);
    }
}
