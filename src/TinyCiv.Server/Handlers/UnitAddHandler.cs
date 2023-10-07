using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;

namespace TinyCiv.Server.Handlers;

public class UnitAddHandler : ClientHandler<CreateUnitClientEvent>
{
    private readonly IMapService _mapService;
    private readonly IInteractableObjectService _interactableObjectService;

    public UnitAddHandler(IMapService mapService, IInteractableObjectService interactableObjectService, ILogger<UnitAddHandler> logger) : base(logger)
    {
        _mapService = mapService;
        _interactableObjectService = interactableObjectService;
    }
    
    protected override async Task OnHandleAsync(CreateUnitClientEvent @event)
    {
        var unit = _mapService.CreateUnit(@event.PlayerId, new ServerPosition { X = @event.X, Y = @event.Y });
        if (unit == null)
        {
            return;
        }

        Task? interactableNotifierTask = null;
        if (unit.IsInteractable())
        {
            var interactable = _interactableObjectService.Initialize(unit);
            
            interactableNotifierTask = NotifyAllAsync(Constants.Server.SendInteractableObject,
                new InteractableObjectServerEvent(unit.Id, interactable.Health, interactable.AttackDamage));
        }
        
        await NotifyCallerAsync(Constants.Server.SendCreatedUnit, new CreateUnitServerEvent(unit))
            .ConfigureAwait(false);

        // Do not notify about map change before created unit receives properties such as Health, AttackDamage
        if (interactableNotifierTask != null)
        {
            await interactableNotifierTask;
        }
        
        await NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!))
            .ConfigureAwait(false);
    }
}