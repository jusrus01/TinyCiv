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

        if (obj.Type != GameObjectType.Warrior)
        {
            throw new NotSupportedException("Only warriors are supported at the moment.");
        }

        if (_objects.ContainsKey(obj.Id))
        {
            return _objects[obj.Id];
        }

        // TODO: export to constant
        var interactableGeneric = new InteractableGeneric
        {
            AttackDamage = 40,
            Health = 100,
            AttackRateInMilliseconds = 2000
        };
        
        _objects.TryAdd(obj.Id, interactableGeneric);

        return interactableGeneric;
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