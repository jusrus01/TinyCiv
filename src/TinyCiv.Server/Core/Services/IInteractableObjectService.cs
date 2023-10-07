using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IInteractableObjectService
{
    IInteractableObject Initialize(ServerGameObject obj);

    IInteractableObject? Get(Guid id);

    void Remove(Guid id);

    bool IsAlive(IInteractableObject obj);
}