using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class PlaceTownHandler : ClientHandler<PlaceTownClientEvent>
{
    private readonly IGameService _gameService;
    
    public PlaceTownHandler(ILogger<IClientHandler> logger, IGameService gameService, IPublisher publisher) : base(publisher, logger)
    {
        _gameService = gameService;
    }

    protected override async Task OnHandleAsync(PlaceTownClientEvent @event)
    {
        // TODO: Clarify if should be possible to place town after it was destroyed.
        // Also, if game ends after town is destroyed, then it does not matter :)
        
        // For now allowing town creation after it was destroyed.
        var townResponse = _gameService.PlaceTown(@event.PlayerId);
        if (townResponse?.Map == null)
        {
            return;
        }
        
        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(townResponse.Map))
            .ConfigureAwait(false);

        var interactableEvent = townResponse.Events?.SingleOrDefault(e => e is InteractableObjectServerEvent);
        if (interactableEvent != null)
        {
            await NotifyAllAsync(Constants.Server.SendInteractableObjectChangesToAll, (InteractableObjectServerEvent)interactableEvent)
                .ConfigureAwait(false);
        }
    }
}
