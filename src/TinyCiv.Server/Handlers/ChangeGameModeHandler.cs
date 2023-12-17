using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Handlers;

public class ChangeGameModeHandler : ClientHandler<ChangeGameModeClientEvent>
{
    private readonly IConnectionIdAccessor _accessor;

    public ChangeGameModeHandler(
        IPublisher publisher,
        IGameService gameService,
        IConnectionIdAccessor accessor,
        ILogger<IClientHandler> logger) : base(publisher, gameService, logger)
    {
        _accessor = accessor;
    }

    protected override async Task OnHandleAsync(ChangeGameModeClientEvent @event)
    {
        void onGamemodeReset()
        {
            NotifyAllAsync(Constants.Server.SendGameModeChangeEventToAll, new GameModeChangeServerEvent(GameModeType.Normal, _accessor.ConnectionId))
                .ConfigureAwait(false);
        }

        var response = GameService.SetGameMode(@event.PlayerId, @event.GameModeType, onGamemodeReset);

        if (response == false)
        {
            return;
        } 

        await NotifyAllAsync(Constants.Server.SendGameModeChangeEventToAll, new GameModeChangeServerEvent(@event.GameModeType, _accessor.ConnectionId))
            .ConfigureAwait(false);
    }
}
