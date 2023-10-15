using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.InteractableObjects;

public interface IInteractableController
{
    void Attack(IInteractableObject? interactable);
    bool IsAlive();
    bool IsUnderAttack(ServerGameObject? obj);
    Task WaitAsync();
}