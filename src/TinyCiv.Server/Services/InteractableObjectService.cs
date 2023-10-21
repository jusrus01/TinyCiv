using System.Collections.Concurrent;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class InteractableObjectService : IInteractableObjectService
{
    private readonly ILogger<InteractableObjectService> _logger;
    private readonly IMapService _mapService;

    private readonly ConcurrentDictionary<Guid, IInteractableObject> _clonedObjects = new();
    private readonly ConcurrentDictionary<Guid, IInteractableObject> _objects = new();

    public InteractableObjectService(IMapService mapService, ILogger<InteractableObjectService> logger)
    {
        _logger = logger;
        _mapService = mapService;
    }
    
    public IInteractableObject Initialize(ServerGameObject obj)
    {
        if (!obj.IsInteractable())
        {
            throw new InvalidOperationException();
        }

        if (_objects.ContainsKey(obj.Id))
        {
            return _objects[obj.Id];
        }
        
        var interactable = ResolveInteractable(obj);
        
        _objects.TryAdd(obj.Id, interactable);

        return interactable;
    }

    public IInteractableInfo? GetInfo(GameObjectType type)
    {
        try
        {
            var dummyInteractable = ResolveInteractable(new ServerGameObject { Type = type });
            return dummyInteractable;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong");
            return null;
        }
    }

    private static IInteractableObject ResolveInteractable(ServerGameObject obj)
    {
        return obj.Type switch
        {
            GameObjectType.Warrior => new InteractableWarrior { GameObjectReferenceId = obj.Id },
            GameObjectType.Cavalry => new InteractableCavalry { GameObjectReferenceId = obj.Id },
            GameObjectType.Tarran => new InteractableTarran { GameObjectReferenceId = obj.Id },
            GameObjectType.City => new InteractableCity { GameObjectReferenceId = obj.Id },
            _ => throw new NotSupportedException()
        };
    }

    public IInteractableObject? Get(Guid id)
    {
        _objects.TryGetValue(id, out var obj);
        return obj;
    }

    public void RegisterClone(IInteractableObject objClone)
    {
        _clonedObjects.TryAdd(Guid.NewGuid(), objClone);
    }

    public IEnumerable<IInteractableObject> FlushClones()
    {
        var clones = _clonedObjects
            .Select(clone => clone.Value)
            .ToList();
        
        _clonedObjects.Clear();

        return clones;
    }

    public void Remove(Guid id)
    {
        _objects.Remove(id, out _);
    }

    public async Task TransformClonesToGameObjectsAsync(
        IEnumerable<IInteractableObject> clones,
        Func<Map, Task> mapChangeNotifier,
        Func<IInteractableObject, Task> attackStateNotifier,
        Func<ServerGameObject, Task> newUnitNotifier)
    {
        ArgumentNullException.ThrowIfNull(clones);
        // interactables should have unchanged reference to existing game objects.
        // otherwise, the clone should not be created as it is - dead already

        var initializedClones = new List<IInteractableObject>();
        
        foreach (var clone in clones)
        {
            var existingUnit = _mapService.GetUnit(clone.GameObjectReferenceId);
            if (existingUnit == null)
            {
                continue;
            }

            var position = _mapService.TryFindClosestAvailablePosition(existingUnit.Position);
            if (position == null)
            {
                continue;
            }

            var createdGameObject = _mapService.CreateUnit(existingUnit.OwnerPlayerId, position, existingUnit.Type);
            if (createdGameObject == null)
            {
                continue;
            }
            
            // NOTE: client does not use "newUnitNotifier"
            await newUnitNotifier(createdGameObject).ConfigureAwait(false);
            
            initializedClones.Add(Initialize(createdGameObject));
        }

        // map update
        await mapChangeNotifier(_mapService.GetMap() ?? throw new InvalidOperationException("Map should be loaded"));

        // only then - interactable update
        var interactableChangesTasks = initializedClones.Select(attackStateNotifier);
        await Task.WhenAll(interactableChangesTasks);
    }
}