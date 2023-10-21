using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IInteractableObjectService
{
    IInteractableObject Initialize(ServerGameObject obj);
    IInteractableInfo? GetInfo(GameObjectType type);

    IInteractableObject? Get(Guid id);

    void RegisterClone(IInteractableObject objClone);
    IEnumerable<IInteractableObject> FlushClones();

    void Remove(Guid id);
    
    Task TransformClonesToGameObjectsAsync(
        IEnumerable<IInteractableObject> clones,
        Func<Map, Task> mapChangeNotifier,
        Func<IInteractableObject, Task> attackStateNotifier,
        Func<ServerGameObject, Task> newUnitNotifier);
}