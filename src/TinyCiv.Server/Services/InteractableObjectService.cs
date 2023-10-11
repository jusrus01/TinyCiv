using System.Collections.Concurrent;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class InteractableObjectService : IInteractableObjectService
{
    private readonly ConcurrentDictionary<Guid, IInteractableObject> _objects = new();

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
        
        var interactable = ResolveInteractable(obj.Type);
        
        _objects.TryAdd(obj.Id, interactable);

        return interactable;
    }

    private static IInteractableObject ResolveInteractable(GameObjectType objType)
    {
        return objType switch
        {
            GameObjectType.Warrior => new InteractableWarrior(),
            GameObjectType.Colonist => new InteractableColonist(),
            GameObjectType.Cavalry => new InteractableCavalry(),
            GameObjectType.Tarran => new InteractableTarran(),
            _ => throw new NotSupportedException()
        };
    }

    public IInteractableObject? Get(Guid id)
    {
        _objects.TryGetValue(id, out var obj);
        return obj;
    }

    public void Remove(Guid id)
    {
        _objects.Remove(id, out _);
    }

    public bool IsAlive(IInteractableObject obj)
    {
        return obj.Health > 0;
    }
}