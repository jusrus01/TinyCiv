using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Dtos.Units;

public record AttackUnitRequest(Guid AttackerId, Guid OpponentId, Func<Map, Task> MapChangeNotifier, Func<IInteractableObject, Task> InteractableObjectStateChangeNotifier);