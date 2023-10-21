using System.Collections.Concurrent;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class InteractableObjectService : IInteractableObjectService
{
    private readonly ILogger<InteractableObjectService> _logger;
    
    private readonly ConcurrentDictionary<Guid, IInteractableObject> _clonedObjects = new();
    
    private readonly ConcurrentDictionary<Guid, IInteractableObject> _objects = new();

    public InteractableObjectService(ILogger<InteractableObjectService> logger)
    {
        _logger = logger;
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
}