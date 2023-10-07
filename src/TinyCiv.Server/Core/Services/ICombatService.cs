using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ICombatService
{
    Task InitiateCombatAsync(
        Guid attackerId,
        Guid opponentId,
        Func<Map, Task> mapChangeNotifier,
        Func<IInteractableObject, Task> attackStateNotifier);
}